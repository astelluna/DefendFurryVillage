using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //현재 게임안에 어떤 플레이어가 존재하는지에 대한 정보를 모아둔다.
    public static PlayerManager Instance { get; private set; }

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
    //현재 플레이어 리스트
    private List<Player> playerList = new List<Player>();
    public void AddPlayer(Player ply)
    {
        playerList.Add(ply);
    }
    public List<Player> GetHaveAugmentPlayerList()
    {
        return playerList.FindAll(n => n.HaveRemainWeaponAugment());
    }
}
