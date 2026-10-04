using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : MonoBehaviour
{
    //현재 게임안에 어떤 플레이어가 존재하는지에 대한 정보를 모아둔다.
    public static CharacterManager Instance { get; private set; }

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
    List<string> selectedCharacterList = new List<string>();

    //런타임중에 소환될수있는 캐릭터, 현재 소환중인 캐릭터 리스트
    //private List<CharacterDataSO> runtimeCharacterPrefabList = new List<CharacterDataSO>();
    private List<Character> unSpawnedCharacterList = new List<Character>();
    private List<Character> spawnedCharacterList = new List<Character>();

    //스폰위치 전용 스크립트 
    [SerializeField] CharacterSpawnPosition characterSpawnPosition;

    //외부에서 플레이어 매니저 초기화 작업 끝났는지 확인하는 Task
    //private Task initalizeCharacterTask;
    private readonly TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
    public Task InitalizeCharacterTask => initTcs.Task;

    //이벤트
    public event Action<Character> OnCharacterSpawnEnd; //플레이어 소환 종료

    //SO
    private readonly Dictionary<string, AsyncOperationHandle> handles = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnEnable()
    {
        //추후에 Awake로 바꿔도될듯
        //Test1 -> 처음 화면에서 캐릭터 데이터 SO를 가져온다.
        
    }
    private void Start()
    {
        Invoke(nameof(TestDelayTaskStart), 3f);

        //Test2 -> 런타임에서 총알을 미리 생성해둔다.
        //initalizeCharacterTask = SetCharactersInit(); // -> 이거 런타임 시작되면 인게임 씬에서 하도록 해야함


    }
    private async Task TestDelayTaskStart()
    {
        try
        {
            await LoadCharacterDataSO();
            initTcs.TrySetResult(true); 
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

    private async Task LoadCharacterDataSO()
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

            //캐릭터 프리팹을 불러와야함.
            Character characterOrigin = await AddressableLoadFunc<Character>(characterDataSO.CharacterReferencePrefab);

            //하나 프리팹을 만들고. 그것을 미리 생성해두자. 
            Character character = Instantiate(characterOrigin, this.transform);
            character.CharacterActive(false);
            unSpawnedCharacterList.Add(character);
            //캐릭터 초기화 및 풀 생성
            character.SetCharacterSO(characterDataSO);
            await character.Init();
            character.MakeWeaponProjectilePool();
            //리스트와 딕셔너리에 넣기
            haveCharacterSOList.Add(characterDataSO);
            haveCharacterSoDict.Add(characterDataSO.CharacterId, characterDataSO);
        }
    }

    public async void SpawnCharacter(Character characterPrefab)
    {
        Transform spawnTrf = characterSpawnPosition.GetTransform(spawnedCharacterList.Count);
        //Character characterPrefab = await AddressableLoadFunc(characterSO.CharacterRefecrencePrefab);

        //Character characterObj =  Instantiate(characterPrefab, spawnTrf.position, spawnTrf.rotation, this.transform); //플레이어는 그냥 생성해주자
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
