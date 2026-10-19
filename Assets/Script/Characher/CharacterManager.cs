using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : Singleton<CharacterManager>
{
    //모든 플레이어 데이터를 어드레서블로 가지고있고 첫 로드해서 allCharacterSOList에 넣어서 사용
    [SerializeField] List<AssetReferenceT<CharacterDataSO>> characterAddressableSOList = new List<AssetReferenceT<CharacterDataSO>>();
    //전체 캐릭터 목록
    List<CharacterDataSO> allCharacterSOList = new List<CharacterDataSO>();
    Dictionary<string, CharacterDataSO> allCharacterSODict = new Dictionary<string, CharacterDataSO>(); //검색용 딕셔너리
    public IReadOnlyList<CharacterDataSO> AllCharacterSOList => allCharacterSOList; //전체 캐릭터를 편하게 리스트로 받아올때 사용
    public IReadOnlyDictionary<string, CharacterDataSO> AllCharacterSODict => allCharacterSODict;

    //플레이어가 가지고있는 캐릭터 목록으로 저장된 딕셔너리
    //List<CharacterDataSO> haveCharacterSOList = new List<CharacterDataSO>();
    //public IReadOnlyList<CharacterDataSO> HaveCharacterSOList => haveCharacterSOList;
    //Dictionary<string, CharacterDataSO> haveCharacterSoDict = new Dictionary<string, CharacterDataSO>(); // TODO. 추후 계정에서 확인해서 캐릭터 가지고있는지 아닌지 체크해서 넣어둘거임

    //캐릭터를 가지고있는지 딕셔너리 (DB에서 읽어서 Set됨)
    Dictionary<string, CharacterInstance> haveCharacterDict = new Dictionary<string, CharacterInstance>();
    public IReadOnlyDictionary<string, CharacterInstance> HaveCharacterDict => haveCharacterDict;


    //선택한 캐릭터 아이디 목록
    List<CharacterDataSO> selectedCharacterList = new List<CharacterDataSO>();

    //런타임중에 소환될수있는 캐릭터, 현재 소환중인 캐릭터 리스트
    public bool HasSelectedData => haveCharacterDict.Count > 0;
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

    //어드레서블 핸들
    private readonly Dictionary<string, CharacterAddressableHandle> characterAddressableHandleDict = new Dictionary<string, CharacterAddressableHandle>();

    public void AddHaveCharacterInstance(CharacterInstance characterInstance)
    {
        if(haveCharacterDict.ContainsKey(characterInstance.characterData.CharacterId))
        {
            MyLog.LogError($"이미 캐릭터가 존재하는데 또 추가하려했습니다. 뭔가에러");
            return;
        }
        haveCharacterDict.Add(characterInstance.characterData.CharacterId, characterInstance);
    }

    //TODO. 그냥 인게임에서 사용하는 필드, 메서드들을 따로 클래스 파서 관리하는것도 좋을듯. 초기화해야할때 new 하기?

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

    public async Task LoadInitFor_InGame()
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
    public void ReleaseFor_InGame()
    {
        //인게임씬에서 어드레서블 로드한 항목에 대해서 릴리즈 진행
        for (int i = 0; i < selectedCharacterList.Count; i++)
        {
            //캐릭터 프리팹을 불러와야함.
            //Release(selectedCharacterList[i].CharacterReferencePrefab);
            AddressableManager.Instance.Release(selectedCharacterList[i].CharacterReferencePrefab);
        }
        //만들었던 객체들 없애기
        foreach (var character in unSpawnedCharacterList) { if(character != null) Destroy(character.gameObject); }
        foreach (var character in spawnedCharacterList) { if (character != null) Destroy(character.gameObject); }

        //필드들 초기화
        selectedCharacterList.Clear();
        unSpawnedCharacterList.Clear();
        spawnedCharacterList.Clear();
        characterSpawnPosition = null;
    }
    private async Task LoadSelectedCharacter_TestMode()
    {
        MyLog.Log("!테스트 모드로 동작합니다!");
        selectedCharacterList = new List<CharacterDataSO>(testSelectedCharacterSOList);
        await LoadInitFor_InGame();
    }

    private async Task SetAndLoadSelectedCharacterData(List<CharacterDataSO> dataList = null)
    {
        //인게임 전에 이 코드를 실행.
        for(int i=0; i< dataList.Count; i++)
        {
            //가지고있는 캐릭터인지 체크
            if (!haveCharacterDict.ContainsKey(dataList[i].CharacterId))
            {
                MyLog.LogError($"가지고있는 캐릭터가 아닌데 선택되어서 인게임에 진입했습니다. 에러발생");
                continue;
            }
            //SO에 프리팹이 있는지 확인
            if (dataList[i].CharacterReferencePrefab == null)
            {
                MyLog.LogError("캐릭터So에 프리팹이 Null 입니다");
                continue;
            }

            //캐릭터 프리팹을 불러와야함.
            Character characterOrigin = await AddressableManager.Instance.GetAssetAsync_Asset<Character>(dataList[i].CharacterReferencePrefab);
            //Character characterOrigin = await AddressableLoadFunc<Character>(dataList[i].CharacterReferencePrefab);

            //하나 프리팹을 만들고. 그것을 미리 생성해두자. 
            Transform spawnTrfParent = characterSpawnPosition.GetTransform(i);
            Character character = Instantiate(characterOrigin, spawnTrfParent);
            character.CharacterActive(false);
            unSpawnedCharacterList.Add(character);
            //캐릭터 초기화 및 풀 생성
            character.SetCharacterSO(dataList[i]);
            await character.Init();
            character.MakeWeaponProjectilePool();
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

    #region 캐릭터 장비 관련 
    public CharacterEquipment GetCharacterEquipment(string characterId)
    {
        //캐릭터별 어떤 장비를 장착중인지 런타임 클래스를 가져온다
        if(!haveCharacterDict.ContainsKey(characterId))
        {
            return null;
        }
        CharacterInstance characterInstance = haveCharacterDict[characterId];
        return characterInstance.characterEquipment;
    }
    #endregion
}
