using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //현재 게임안에 어떤 플레이어가 존재하는지에 대한 정보를 모아둔다.
    public static PlayerManager Instance { get; private set; }

    //현재 플레이어 리스트
    private List<Player> spawnedPlayerList = new List<Player>();
    public IReadOnlyList<Player> SpawnerdPlayerList => spawnedPlayerList;

    //테스트용. 추후에 어떤 캐릭터가 등장하는지 다른곳에 등록을 해두자
    public List<Player> testAvailablePlayerList = new List<Player>();

    //스폰위치 전용 스크립트 
    [SerializeField] PlayerSpawnPosition playerSpawnPosition;

    //외부에서 플레이어 매니저 초기화 작업 끝났는지 확인하는 Task
    private Task initalizePlayerTask;
    public Task InitalizePlayerTask => initalizePlayerTask;

    //이벤트
    public event Action<Player> OnPlayerSpawnEnd; //플레이어 소환 종료

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        //Test -> 총알을 미리 생성해둔다.
        initalizePlayerTask = SetPlayersInit();
    }
    private async Task SetPlayersInit()
    {
        //총알 풀과, 어드레서블을 통한 플레이어 DataSO와 플레이어 안의 무기SO를 미리 등록.
        foreach(Player player in testAvailablePlayerList)
        {
            player.MakeWeaponProjectilePool();
            await player.Init_Preload();
        }
    }


    private void Update()
    {
        int k = 20;
    }


    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    
    public async void SpawnPlayer(Player playerPrefab)
    {
        Transform spawnTrf = playerSpawnPosition.GetTransform(spawnedPlayerList.Count);
        Player playerObj =  Instantiate(playerPrefab, spawnTrf.position, spawnTrf.rotation, this.transform); //플레이어는 그냥 생성해주자
        spawnedPlayerList.Add(playerObj);
        await playerObj.Init();

        OnPlayerSpawnEnd?.Invoke(playerObj);
    }
    public List<Player> GetHaveAugmentPlayerList()
    {
        return spawnedPlayerList.FindAll(n => n.HaveRemainWeaponAugment());
    }
    public List<Player> GetRemainPlayer()
    {
        List<Player> remainPlayerList = new List<Player>();

        foreach(var curPlayer in testAvailablePlayerList)
        {
            bool isSame = false;
            foreach(var spawnedPlayer in spawnedPlayerList)
            {
                if (curPlayer.ComparePlayerSame(spawnedPlayer))
                {
                    isSame = true;
                    break;
                }
            }
            if(!isSame)
            {
                remainPlayerList.Add(curPlayer);
            }
        }
        return remainPlayerList;
    }
    public bool HavePlayer()
    {
        if (spawnedPlayerList.Count == 0) return false;
        return true;
    }
    
}
