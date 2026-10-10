using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

/// <summary>
/// Preload + 런타임 로드를 하나의 캐시로 통합한 어드레서블 매니저.
///  - Preload 라벨 에셋: Persistent (게임 종료까지 유지, Release 무시)
///  - 그 외 on-demand 에셋: RefCount 기반 (0이 되면 해제)
///  - 캐시 키 = PrimaryKey + ResourceType (같은 주소에 타입이 여러 개여도 안전)
/// </summary>
public class AddressableManager : Singleton<AddressableManager>
{
    private class Entry
    {
        //할당 해제를 위해 만든 클래스. 할당 해제를 위한 Handle과 참조 횟수, Preload 여부를 파악
        public AsyncOperationHandle Handle;
        public bool Persistent; // Preload로 올라온 것
        public int RefCount;    // on-demand 사용 횟수 (참조횟수)
    }

    // 캐시 키(PrimaryKey|Type) -> 로드된 엔트리
    private readonly Dictionary<string, Entry> loaded = new();

    // 로드 진행 중인 Task (동시 호출 중복 로드 방지)
    private readonly Dictionary<string, Task<Entry>> loading = new();

    // RuntimeKey(GUID)|Type -> Location (동기 조회 결과 캐시)
    private readonly Dictionary<string, IResourceLocation> locationCache = new();

    private Task preloadTask = Task.CompletedTask;

    protected override async void OnAwake()
    {
        preloadTask = PreloadAsync("Preload");
        await preloadTask;
    }

    private void OnDestroy() => ReleaseAll();

    // ───────────────────────── Preload ─────────────────────────

    /// 로딩 화면 등에서 Preload 완료를 기다릴 때 사용
    public Task WaitPreloadAsync() => preloadTask;

    public bool IsPreloadDone => preloadTask.IsCompleted;

    public async Task PreloadAsync(string label)
    {
        //태그가 붙어있는것들은 게임시작시 미리 다 불러와둔다.
        try
        {
            //IResourceLocation으로 불러온다. (목록과 타입을 자동으로 분리해서 가져옴)
            var locHandle = Addressables.LoadResourceLocationsAsync(label, typeof(object));
            IList<IResourceLocation> locations = await locHandle.Task;

            List<Task> tasks = new List<Task>(locations.Count); //미리 갯수만큼 Capacity를 잡아둔다(메모리 공간 미리 할당). 요소 갯수는 0임
            //어드레서블 리소스 불러와서 저장.
            foreach (IResourceLocation loc in locations)
                tasks.Add(AcquireAsync(loc, persistent: true, addRef: false));

            Addressables.Release(locHandle);
            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            MyLog.LogError($"Preload 실패 (label: {label}): {e}");
        }
    }

    // ───────────────────────── 비동기 Get ─────────────────────────

    public async Task<T> GetAssetAsync_Asset<T>(AssetReference assetRef) where T : Object
    {
        //1. 만약 프리팹 안의 컴포넌트를 불러왔다면? GO로 변환해서 가져와야함
        if (typeof(Component).IsAssignableFrom(typeof(T)))
        {
            GameObject go = await GetAssetAsync_Object<GameObject>(assetRef);
            if (go == null) return null;
            var comp = go.GetComponent(typeof(T)) as T;   // 제네릭 제약 없이 Type으로 조회
            if (comp == null)
            {
                MyLog.LogError($"프리팹에 {typeof(T).Name} 컴포넌트가 없습니다: {go.name}");
                Release(assetRef);
                return null;
            }
            return comp;
        }
        //2. 일반 에셋이라면 그냥 불러오자
        return await GetAssetAsync_Object<T>(assetRef);
    }


    /// 일반 에셋(Sprite, AudioClip, SO, GameObject 등). 사용이 끝나면 Release 호출 필요
    private async Task<T> GetAssetAsync_Object<T>(AssetReference assetRef) where T : Object
    {
        await preloadTask;
        //찾으려는 에셋이 어드레서블에 등록되어있는지 확인하고 loc를 가져옴
        if (!TryFindLocate(assetRef, typeof(T), out var loc))
        {
            MyLog.LogError($"Addressable 에셋을 찾을 수 없습니다. RuntimeKey: {assetRef?.RuntimeKey}, Type: {typeof(T).Name}");
            return null;
        }
        //어드레서블에 있는지 확인했으니 해당 객체를 가져옴
        Entry entry = await AcquireAsync(loc, persistent: false, addRef: true);
        if (entry == null) return null;

        var result = entry.Handle.Result as T;
        if (result == null)
        {
            //로드된 에셋은 Texture2D인데 Sprite로 요청했다면? 이런경우 에러뜰거임
            MyLog.LogError($"에셋 타입 불일치: {loc.PrimaryKey} -> {typeof(T).Name}");
            ReleaseKey(MakeKey(loc));
            return null;
        }
        return result;
    }
    // ───────────────────────── 동기 Get (Preload 전제) ─────────────────────────

    public bool TryGet<T>(AssetReference assetRef, out T asset) where T : Object
    {
        if(typeof(Component).IsAssignableFrom(typeof(T)))
        {
            asset = null;
            //컴포넌트 형식이고 && 해당 컴포넌트가 실제로 존재하는지도 체크를 해야함
            if (!TryGet_Object<GameObject>(assetRef, out GameObject goAsset)) { return false; }
            return goAsset.TryGetComponent<T>(out asset);
        }
        //bool 
        return TryGet_Object<T>(assetRef, out asset);
    }

    /// 이미 로드된 에셋만 동기로 반환. 없으면 false (로드하지 않음). RefCount 변화 없음.
    private bool TryGet_Object<T>(AssetReference assetRef, out T asset) where T : Object
    {
        asset = null;

        if (typeof(Component).IsAssignableFrom(typeof(T)))
        {
            MyLog.LogError($"{typeof(T).Name}은 컴포넌트입니다. TryGetComponent를 사용하세요.");
            return false;
        }
        //에셋이 어드레서블에 존재하는지 확인
        if (!TryFindLocate(assetRef, typeof(T), out var loc)) return false;
        //Preload로 등록된 에셋이었는지 딕셔너리로 확인해서 있으면 out에 있는 asset에 등록한다.
        if (loaded.TryGetValue(MakeKey(loc), out var e) && e.Handle.IsValid())
            asset = e.Handle.Result as T;
        //asset를 성공적으로 가져왔는지 return을 한다
        return asset != null;
    }

    private bool TryGet_Component<T>(AssetReference assetRef, out T comp) where T : Component
    {
        //추후 없애자. 쓰고있는곳에서 좀 다르게 해야할듯
        //프리로드된 에셋에서 Get을 하고, + 추가로 특정 컴포넌트 형태로 변환해서 바로 가져옴
        comp = null;
        return TryGet_Object<GameObject>(assetRef, out var go) && go.TryGetComponent(out comp);
    }

    /// (테스트용)Preload 전제로 사용. 없으면 에러 로그 (개발 중 라벨 누락 감지용).
    public T GetPreloaded<T>(AssetReference assetRef) where T : Object
    {
        //성공하면 에셋을 바로 가져옴
        if (TryGet_Object<T>(assetRef, out var asset)) return asset;
        //실패하면 에러 로그를 남기고 null 반환
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        MyLog.LogError($"Preload되지 않은 에셋입니다. 'Preload' 라벨을 확인하세요. RuntimeKey: {assetRef?.RuntimeKey}, Type: {typeof(T).Name}");
#endif
        return null;
    }
    /// (테스트용) Component 변환이 가능한지
    public T GetPreloadedComponent<T>(AssetReference assetRef) where T : Component
    {
        //성광하면 에셋을 바로 가져옴
        if (TryGet_Component<T>(assetRef, out var comp)) return comp;
        //실패하면 에러 로그를 남기고 null 반환
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        MyLog.LogError($"Preload되지 않은 프리팹/컴포넌트입니다. RuntimeKey: {assetRef?.RuntimeKey}, Type: {typeof(T).Name}");
#endif
        return null;
    }

    // ───────────────────────── Release ─────────────────────────

    /// 프리팹(GameObject) 해제. 기존 Release(AssetReference)와 동일하게 사용 가능.
    public void Release(AssetReference assetRef) => Release<GameObject>(assetRef);

    /// GetAssetAsync&lt;T&gt;로 얻은 에셋 해제. Persistent(Preload) 에셋은 무시됨.
    public void Release<T>(AssetReference assetRef) where T : Object
    {
        Type loadedType = typeof(Component).IsAssignableFrom(typeof(T)) ? typeof(GameObject) : typeof(T);

        //실제로 어드레서블에 존재하는건지 확인
        if (!TryFindLocate(assetRef, loadedType, out var loc)) return;
        ReleaseKey(MakeKey(loc));
    }

    public void ReleaseAll()
    {
        foreach (var e in loaded.Values)
            if (e.Handle.IsValid()) Addressables.Release(e.Handle);

        loaded.Clear();
        loading.Clear();
        locationCache.Clear();
    }

    private void ReleaseKey(string key)
    {
        //런타임 중에 로드한 것들 모아둔 딕셔너리에 없으면 return;
        if (!loaded.TryGetValue(key, out var e)) return;
        //참조 카운트 한개 제거
        e.RefCount = Mathf.Max(0, e.RefCount - 1);
        //1. 지워져도 되는것 || 쓰는곳이 아무도 없는지 체크
        if (e.Persistent || e.RefCount > 0) return;
        //2. 핸들이 실제로 유효하다면 어드레서블 할당 해제
        if (e.Handle.IsValid()) Addressables.Release(e.Handle);
        loaded.Remove(key);
    }

    // ───────────────────────── 내부 로직 ─────────────────────────

    //키를 만든다. 어드레서블 주소 이름과 타입
    private static string MakeKey(IResourceLocation loc)
        => $"{loc.PrimaryKey}|{loc.ResourceType?.FullName}";

    /// GUID(RuntimeKey) -> Location 을 동기로 해석 (카탈로그는 이미 메모리에 있음)
    private bool TryFindLocate(AssetReference assetRef, Type type, out IResourceLocation loc)
    {
        //에셋의 위치와 어떤 타입를 입력하면 어드레서블에 등록된 객체인지 확인해서 IResourceLocation을 반환함
        loc = null;
        //필드가 비어있지않고, 에셋이 실제로 존재하는지 확인 
        if (assetRef == null || !assetRef.RuntimeKeyIsValid()) return false;

        object runtimeKey = assetRef.RuntimeKey;
        string cacheKey = $"{runtimeKey}|{type.FullName}";

        if (locationCache.TryGetValue(cacheKey, out loc)) return true;
        //어드레서블 전체 주소에서 하나씩 비교해본다.
        foreach (var locator in Addressables.ResourceLocators)
        {
            //Locate로 이 키와 타입에 해당하는 주소를 찾음. 
            if (locator.Locate(runtimeKey, type, out var locs) && locs != null && locs.Count > 0)
            {
                loc = locs[0];
                locationCache[cacheKey] = loc;
                return true;
            }
        }
        return false;
    }


    private async Task<Entry> AcquireAsync(IResourceLocation loc, bool persistent, bool addRef)
    {
        /// 캐시 hit면 즉시 반환, 로드 중이면 같은 Task 공유, 아니면 새로 로드.
        /// 매개변수: Preload할때와 런타임Load할때 다른 동작을 하기 위해서 따로 설정.
        string key = MakeKey(loc);
        Entry entry = null;

        if (loaded.TryGetValue(key, out var cached) && cached.Handle.IsValid())
        {
            entry = cached;
        }
        else if (loading.TryGetValue(key, out var inFlight))
        {
            entry = await inFlight;
        }
        else
        {
            //프리로드에는 없으므로 새로 로드해준다.
            var tcs = new TaskCompletionSource<Entry>();
            loading[key] = tcs.Task;
            try
            {
                AsyncOperationHandle<object> handle = Addressables.LoadAssetAsync<object>(loc);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    entry = new Entry { Handle = handle };
                    loaded[key] = entry;
                }
                else
                {
                    MyLog.LogError($"Addressable 로드 실패: {key}");
                    if (handle.IsValid()) Addressables.Release(handle);
                }
            }
            catch (Exception e)
            {
                MyLog.LogError($"Addressable 로드 예외: {key}\n{e}");
            }
            finally
            {
                loading.Remove(key);
                tcs.SetResult(entry); // 실패 시 null -> 대기 중이던 쪽도 null을 받음
            }
        }

        if (entry == null) return null;

        if (persistent) entry.Persistent = true; // on-demand로 먼저 올라왔어도 Preload면 승격
        if (addRef) entry.RefCount++;
        return entry;
    }
}