using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterManager : MonoBehaviour
{
    //현재 게임안에 어떤 플레이어가 존재하는지에 대한 정보를 모아둔다.
    public static CharacterManager Instance { get; private set; }

    //모든 플레이어 데이터를 어드레서블로 가지고있고 첫 로드해서 allCharacterSOList에 넣어서 사용
    [SerializeField] List<AssetReferenceT<CharacterDataSO>> characterAddressableSOList = new List<AssetReferenceT<CharacterDataSO>>();
    List<CharacterDataSO> allCharacterSOList = new List<CharacterDataSO>();
    Dictionary<string, CharacterDataSO> allCharacterSoDict = new Dictionary<string, CharacterDataSO>();
    public IReadOnlyList<CharacterDataSO> AllCharacterSOList => allCharacterSOList;
    //public IReadOnlyList<CharacterDataSO> CharacterDataSOList => characterDataSOList;

    //선택한 캐릭터 아이디 목록
    List<string> selectedCharacterList = new List<string>();

    //런타임중에 소환될수있는 캐릭터, 현재 소환중인 캐릭터 리스트
    //private List<Character> runtimeCharacterPrefabList = new List<Character>();
    private List<CharacterDataSO> runtimeCharacterPrefabList = new List<CharacterDataSO>();
    private List<Character> spawnedCharacterList = new List<Character>();

    //테스트용. 추후에 어떤 캐릭터가 등장하는지 다른곳에 등록을 해두자
    //public List<Character> testAvailableCharacterList = new List<Character>();

    //스폰위치 전용 스크립트 
    [SerializeField] CharacterSpawnPosition characterSpawnPosition;

    //외부에서 플레이어 매니저 초기화 작업 끝났는지 확인하는 Task
    private Task initalizeCharacterTask;
    public Task InitalizeCharacterTask => initalizeCharacterTask;

    //이벤트
    public event Action<Character> OnCharacterSpawnEnd; //플레이어 소환 종료

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
    private void Start()
    {
        //Test1 -> 처음 화면에서 캐릭터 데이터 SO를 가져온다.
        initalizeCharacterTask = LoadCharacterDataSO();

        //Test2 -> 런타임에서 총알을 미리 생성해둔다.
        //initalizeCharacterTask = SetCharactersInit(); // -> 이거 런타임 시작되면 인게임 씬에서 하도록 해야함

    }


    private async Task LoadCharacterDataSO()
    {
        //어드레서블로 되어있는 캐릭터 데이터들을 계속 불러와본다 
        foreach (var item in characterAddressableSOList)
        {
            CharacterDataSO characterDataSO = await AddressablePreloadManager.Instance.GetAsync<CharacterDataSO>(item);
            allCharacterSOList.Add(characterDataSO);
            if(allCharacterSoDict.ContainsKey(characterDataSO.CharacterId))
            {
                MyLog.LogError("SO에 캐릭터ID가 중복입니다");
                continue;
            }
            //초기화
            characterDataSO.CharacterPrefab.Init_Preload();

            allCharacterSoDict.Add(characterDataSO.CharacterId, characterDataSO);
        }
    }

    private async Task SetRuntimeCharactersInit()
    {
        //TODO. 인게임 진입하면 이 코드 실행해야함
        //인게임에 선택된 캐릭터들 전용 메서드. 총알 풀과, 어드레서블을 통한 플레이어 DataSO와 플레이어 안의 무기SO를 미리 등록.
        //리스트로 일단 하나씩 가져온다.
        runtimeCharacterPrefabList.Clear();
        foreach(string id in selectedCharacterList)
        {
            if (!allCharacterSoDict.ContainsKey(id))
            {
                MyLog.LogError("찾으려는 캐릭터 ID가 없음. DB와 SO 확인 필요");
                continue;
            }
            CharacterDataSO characterSO = allCharacterSoDict[id];
            runtimeCharacterPrefabList.Add(characterSO);
        }

        foreach(var characterSO in runtimeCharacterPrefabList)
        {
            characterSO.CharacterPrefab.MakeWeaponProjectilePool();
        }
    }

    

    //public async void SpawnCharacter(Character characterPrefab)
    public async void SpawnCharacter(CharacterDataSO characterSO)
    {
        Transform spawnTrf = characterSpawnPosition.GetTransform(spawnedCharacterList.Count);
        Character characterObj =  Instantiate(characterSO.CharacterPrefab, spawnTrf.position, spawnTrf.rotation, this.transform); //플레이어는 그냥 생성해주자
        spawnedCharacterList.Add(characterObj);
        characterObj.SetCharacterSO(characterSO);
        await characterObj.Init();
        OnCharacterSpawnEnd?.Invoke(characterObj);
    }
    public List<CharacterDataSO> GetHaveAugmentCharacterList()
    {
        return spawnedCharacterList
            .FindAll(n => n.HaveRemainWeaponAugment())
            .ConvertAll(n => n.CharacterDataSO);
    }
    public List<CharacterDataSO> GetRemainCharacter()
    {
        List<CharacterDataSO> remainCharacterList = new List<CharacterDataSO>();

        foreach(var curCharacterSO in runtimeCharacterPrefabList)
        {
            bool isSame = false;
            foreach(var spawnedCharacter in spawnedCharacterList)
            {
                if(curCharacterSO == spawnedCharacter.CharacterDataSO)
                {
                    isSame = true;
                    break;
                }
            }
            if(!isSame)
            {
                remainCharacterList.Add(curCharacterSO);
            }
        }
        return remainCharacterList;
    }
    public bool HaveCharacter()
    {
        if (spawnedCharacterList.Count == 0) return false;
        return true;
    }
    
}
