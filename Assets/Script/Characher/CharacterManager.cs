using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TextCore.Text;

public class CharacterManager : Singleton<CharacterManager>
{
    //현재 게임안에 어떤 플레이어가 존재하는지에 대한 정보를 모아둔다.
    //public static CharacterManager Instance { get; private set; }

    //모든 플레이어 데이터를 어드레서블로 가지고있고 첫 로드해서 allCharacterSOList에 넣어서 사용
    [SerializeField] List<AssetReferenceT<CharacterDataSO>> characterAddressableSOList = new List<AssetReferenceT<CharacterDataSO>>();
    //전체 캐릭터 목록
    List<CharacterDataSO> allCharacterSOList = new List<CharacterDataSO>();
    public IReadOnlyList<CharacterDataSO> AllCharacterSOList => allCharacterSOList; //전체 캐릭터를 편하게 리스트로 받아올때 사용
    Dictionary<string, CharacterDataSO> allCharacterSODict = new Dictionary<string, CharacterDataSO>(); //검색용 딕셔너리

    //플레이어가 가지고있는 캐릭터 목록으로 저장된 딕셔너리
    List<CharacterDataSO> haveCharacterSOList = new List<CharacterDataSO>();
    public IReadOnlyList<CharacterDataSO> HaveCharacterSOList => haveCharacterSOList;
    Dictionary<string, CharacterDataSO> haveCharacterSoDict = new Dictionary<string, CharacterDataSO>();

    //선택한 캐릭터 아이디 목록
    List<CharacterDataSO> selectedCharacterList = new List<CharacterDataSO>();

    //런타임중에 소환될수있는 캐릭터, 현재 소환중인 캐릭터 리스트
    public bool HasSelectedData => haveCharacterSOList.Count > 0;
    [Header("!테스트용!: 인게임씬에서 테스트할때만 수동으로 넣고. 이외에는 비워두기")]
    [SerializeField] List<CharacterDataSO> testSelectedCharacterSOList = new List<CharacterDataSO>();
    private List<Character> unSpawnedCharacterList = new List<Character>();
    private List<Character> spawnedCharacterList = new List<Character>();

    //스폰위치 전용 스크립트 
    CharacterSpawnPosition characterSpawnPosition;

    //외부에서 플레이어 매니저 초기화 작업 끝났는지 확인하는 Task
    private readonly TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
    public Task InitalizeCharacterTask => initTcs.Task;
    private TaskCompletionSource<bool> ingameCharacterLoadTcs = new TaskCompletionSource<bool>();
    public Task IngameCharacterLoadTask => ingameCharacterLoadTcs.Task;

    //이벤트
    public event Action<Character> OnCharacterSpawnEnd; //플레이어 소환 종료

    //SO
    private readonly Dictionary<string, AsyncOperationHandle> handles = new();


    public void SetSelectedCharacter(List<CharacterDataSO> characterDataSOList)
    {
        selectedCharacterList = characterDataSOList;
    }


    protected override void OnAwake()
    {
        
    }
    private void Start()
    {
        TestDelayTaskStart();
        //Invoke(nameof(TestDelayTaskStart), 3f);

        //인게임 테스트용.(인게임씬에서 실행하면 테스트용으로 실행한다)
        ForIngameTestRoom();
    }

    public async Task ForIngameTestRoom()
    {
        //인게임에서 테스트룸을 위해 사용
        await InitalizeCharacterTask;
        if (HasSelectedData) return;
#if UNITY_EDITOR
        if (testSelectedCharacterSOList.Count == 0)
        {
            MyLog.LogError("테스트 모드가 아닌지 확인해주세요. 테스트 캐릭터가 비어있습니다. CharacterManager의 testCharacterSOList를 채워주세요.");
            return;
        }
        MyLog.Log("[TestRoom] 로비를 거치지 않아 테스트 캐릭터로 시작합니다.");
        await LoadSelectedCharacter_TestMode();
#else
    MyLog.LogError("선택된 캐릭터 데이터가 없습니다.");
#endif
    }

    private void ClearIngameState()
    {
        //TODO. 캐릭터들 만든거 스폰해제해야함. 어드레서블도
        unSpawnedCharacterList.Clear();
        spawnedCharacterList.Clear();
        haveCharacterSOList.Clear();
        haveCharacterSoDict.Clear();
    }

    private async Task TestDelayTaskStart()
    {
        try
        {
            await LoadCharacterDataSO_Init();
            initTcs.TrySetResult(true);
            MyLog.Log($"캐릭터 정보 설정 완료");
        }
        catch(Exception e)
        {
            MyLog.LogError($"캐릭터 초기화 실패:{e}");
            initTcs.TrySetException(e);
        }
    }

    public async Task<T> AddressableLoadFunc<T>(AssetReference assetRef) where T : UnityEngine.Object
    {
        string key = assetRef.RuntimeKey.ToString();

        if (typeof(Component).IsAssignableFrom(typeof(T)))
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(assetRef);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                MyLog.LogError($"프리팹 로드 실패: {key}");
                Addressables.Release(handle);
                return null;
            }

            var comp = handle.Result.GetComponent<T>();
            if (comp == null)
            {
                MyLog.LogError($"프리팹에 {typeof(T).Name} 컴포넌트가 없습니다: {handle.Result.name}");
                Addressables.Release(handle);
                return null;
            }

            handles[key] = handle; // 나중에 Release 하기 위해 저장
            return comp;
        }

        var h = Addressables.LoadAssetAsync<T>(assetRef);
        await h.Task;
        if (h.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(h);
            return null;
        }

        handles[key] = h;
        return h.Result;
    }
    public void Release(AssetReference assetReference)
    {
        string key = assetReference.RuntimeKey.ToString();
        if(handles.TryGetValue(key, out var handle ))
        {
            Addressables.Release(handle);
            handles.Remove(key);
        }
    }

    private async Task LoadCharacterDataSO_Init()
    {
        //어드레서블로 되어있는 캐릭터 데이터들을 계속 불러와본다 
        foreach (var item in characterAddressableSOList)
        {
            CharacterDataSO characterDataSO = await AddressablePreloadManager.Instance.GetAsync<CharacterDataSO>(item);
            if(allCharacterSODict.ContainsKey(characterDataSO.CharacterId))
            {
                MyLog.LogError("SO에 캐릭터ID가 중복입니다");
                continue;
            }
            //초기화
            allCharacterSOList.Add(characterDataSO);
            allCharacterSODict.Add(characterDataSO.CharacterId, characterDataSO);


        }
    }
    public void SetCharacterSpawnPos(CharacterSpawnPosition spawnPos)
    {
        characterSpawnPosition = spawnPos;
    }

    public async Task LoadSelectedCharacter()
    {
        ingameCharacterLoadTcs = new TaskCompletionSource<bool>();
        TaskCompletionSource<bool> tcs = ingameCharacterLoadTcs; //이렇게 해야 다른곳에서 await한게 아닌 이번에 새로생성된거로 사용함 
        try
        {
            await SetAndLoadSelectedCharacterData(selectedCharacterList);
            tcs.SetResult(true);
        }
        catch(Exception e)
        {
            MyLog.LogError($"인게임 캐릭터 로드 실패: {e}");
            tcs.SetException(e);
            throw e;
        }
    }
    private async Task LoadSelectedCharacter_TestMode()
    {
        MyLog.Log("!테스트 모드로 동작합니다!");
        selectedCharacterList = testSelectedCharacterSOList;
        await LoadSelectedCharacter();
    }

    private async Task SetAndLoadSelectedCharacterData(List<CharacterDataSO> dataList = null)
    {
        //인게임 전에 이 코드를 실행.
        for(int i=0; i< dataList.Count; i++)
        {
            if (dataList[i].CharacterReferencePrefab == null)
            {
                MyLog.LogError("캐릭터So에 프리팹이 Null 입니다");
            }

            //캐릭터 프리팹을 불러와야함.
            Character characterOrigin = await AddressableLoadFunc<Character>(dataList[i].CharacterReferencePrefab);

            //하나 프리팹을 만들고. 그것을 미리 생성해두자. 
            Transform spawnTrfParent = characterSpawnPosition.GetTransform(i);
            Character character = Instantiate(characterOrigin, spawnTrfParent);
            character.CharacterActive(false);
            unSpawnedCharacterList.Add(character);
            //캐릭터 초기화 및 풀 생성
            character.SetCharacterSO(dataList[i]);
            await character.Init();
            character.MakeWeaponProjectilePool();
            //리스트와 딕셔너리에 넣기
            haveCharacterSOList.Add(dataList[i]);
            haveCharacterSoDict.Add(dataList[i].CharacterId, dataList[i]);
        }
    }

    public async void SpawnCharacter(Character characterPrefab)
    {
        Transform spawnTrf = characterSpawnPosition.GetTransform(spawnedCharacterList.Count);
        foreach( var item in unSpawnedCharacterList)
        {
            if(item.CharacterDataSO.CharacterId == characterPrefab.CharacterDataSO.CharacterId)
            {
                Character characterObj = item;
                characterObj.transform.position = spawnTrf.position;
                characterObj.transform.rotation = spawnTrf.rotation;
                characterObj.CharacterActive(true);
                unSpawnedCharacterList.Remove(characterObj);
                spawnedCharacterList.Add(characterObj);
                OnCharacterSpawnEnd?.Invoke(characterObj);
                break;
            }
        }
    }
    public List<Character> GetHaveAugmentCharacterList()
    {
        return spawnedCharacterList
            .FindAll(n => n.HaveRemainWeaponAugment());
    }
    public List<Character> GetRemainCharacter()
    {
        List<Character> remainCharacterList = new List<Character>();
        foreach(var item in unSpawnedCharacterList)
        {
            remainCharacterList.Add(item);
        }
        return remainCharacterList;
    }
    public bool HaveCharacter()
    {
        if (spawnedCharacterList.Count == 0) return false;
        return true;
    }


}
