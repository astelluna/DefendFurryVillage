using System.Collections.Generic;
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
        foreach(Player player in testAvailablePlayerList)
        {
            player.MakeWeaponProjectilePool();
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
    
    public void SpawnPlayer(Player playerPrefab)
    {
        Transform spawnTrf = playerSpawnPosition.GetTransform(spawnedPlayerList.Count);
        Player playerObj =  Instantiate(playerPrefab, spawnTrf.position, spawnTrf.rotation, this.transform); //플레이어는 그냥 생성해주자
        spawnedPlayerList.Add(playerObj);

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
