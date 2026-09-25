using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
public class AddressablePreloadManager : Singleton<AddressablePreloadManager>
{    
    //GUID(RuntimeKey) : 에셋의 고유 ID 
    //PrimaryKey : 에셋을 찾을떄 사용하는 대표키

    //어드레서블의 항목중 'Preload' 태그가 담긴 항목들에 대해서 미리 load 해둔다. 
    //딕셔너리를 통해 Load한 어드레서블 항목에 대해서 미리 저장해두고, 반환할수있도록 하자. 
    // key: 실제 Addressables PrimaryKey (Address) -> 로드된 핸들
    private readonly Dictionary<string, AsyncOperationHandle> assetDict = new();

    // key: AssetReference의 GUID(RuntimeKey) -> 실제 PrimaryKey
    // Address가 GUID와 다르게 커스텀 지정된 경우를 위한 보정 캐시
    // 필요한이유: Primary키와 GUID(RuntimeKey)는 다르다. 이거는 라벨로 전체 Handle리스트를 가져오려면 PrimaryKey로 처리해야함
    private readonly Dictionary<string, string> guidToPrimaryKey = new();

    //프리로드 대기
    Task allPreloadEnd;
    //public bool AllPreloadEnd => allPreloadEnd != null && allPreloadEnd.IsCompleted;

    public async Task PreloadAll(string label)
    {
        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
        Addressables.LoadResourceLocationsAsync(label, typeof(object)); // 타입 제한 없음
        IList<IResourceLocation> locations = await locationsHandle.Task;

        foreach (IResourceLocation loc in locations)
        {
            string key = loc.PrimaryKey;
            if (assetDict.ContainsKey(key)) continue;

            // 제네릭 대신 object로 로드
            AsyncOperationHandle<object> handle = Addressables.LoadAssetAsync<object>(loc);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                assetDict[key] = handle;
            else
                Addressables.Release(handle);
        }

        Addressables.Release(locationsHandle);
    }

    /// <summary>
    /// 완전히 안전하게 매칭. Preload 시 Address를 커스텀으로 바꿨어도 100% 찾음.
    /// 최초 1회만 내부적으로 비동기 조회 후 캐싱, 이후로는 캐시 hit.
    /// </summary>
    public async Task<T> GetAsync<T>(AssetReferenceT<T> assetRef) where T : UnityEngine.Object
    {
        await allPreloadEnd;

        string guid = assetRef.RuntimeKey.ToString();

        // 1) 이미 guid 자체가 PrimaryKey인 경우 (Address 안 바꾼 기본 케이스)
        if (assetDict.TryGetValue(guid, out var directHandle) && directHandle.IsValid())
            return directHandle.Result as T;

        // 2) 이전에 이미 guid -> primaryKey 매핑을 캐싱해둔 경우
        if (guidToPrimaryKey.TryGetValue(guid, out var cachedKey)
            && assetDict.TryGetValue(cachedKey, out var cachedHandle)
            && cachedHandle.IsValid())
        {
            return cachedHandle.Result as T; //object타입인 Result를 T타입으로 변경해서 반환해준다
        }

        // 3) 둘 다 실패하면 최초 1회 비동기로 실제 PrimaryKey를 확인 (Address 커스텀 대응)
        AsyncOperationHandle<IList<IResourceLocation>> locHandle = Addressables.LoadResourceLocationsAsync(assetRef.RuntimeKey);
        IList<IResourceLocation> locations = await locHandle.Task;
        string primaryKey = locations.Count > 0 ? locations[0].PrimaryKey : null;
        Addressables.Release(locHandle);

        if (primaryKey == null) return null;

        guidToPrimaryKey[guid] = primaryKey; // 다음부터는 캐시로 즉시 매칭

        if (assetDict.TryGetValue(primaryKey, out var handle) && handle.IsValid())
            return handle.Result as T;
        MyLog.LogError($"Addressable 에셋을 찾을 수 없습니다. RuntimeKey: {guid}, PrimaryKey: {primaryKey}");

        return null;
    }

    public void ReleaseAll()
    {
        foreach (var h in assetDict.Values)
        {
            if (h.IsValid())
                Addressables.Release(h);
        }
        assetDict.Clear();
        guidToPrimaryKey.Clear();
    }
    protected override async void OnAwake()
    {
        allPreloadEnd = PreloadAll("Preload");
        await allPreloadEnd;
    }

    private void OnDestroy()
    {
        ReleaseAll();
    }


}
