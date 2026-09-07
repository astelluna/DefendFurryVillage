using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }

    //현재 플레이어 리스트
    private List<Player> playerList = new List<Player>();

    [SerializeField] AugmentPanel augmentPanel;

    List<WeaponAugment> selectedAugmentList = new List<WeaponAugment>();
    List<Player> haveAugmentPlyerList = new List<Player>();
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
        //TEST
        Invoke( nameof(ShowAugmentUI), 3.0f);
        //ShowAugmentUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        ClearSelectAugmentList();
    }

    public void AddPlayer(Player ply)
    {
        playerList.Add(ply);
    }
    public void ShowAugmentUI()
    {
        //UI를 보여주고 현재 플레이어중 1개를 선택해서 쭉쭉 내려간다음에 증강 1개를 선택해서 가져온다.
        bool needShowPanel = false;
        selectedAugmentList.Clear();
        
        for(int i=0; i< 3; i++)
        {
            haveAugmentPlyerList.Clear();
            haveAugmentPlyerList = playerList.FindAll(n => n.HaveRemainWeaponAugment());
            if (haveAugmentPlyerList.Count == 0)
            {
                MyLog.LogWarning("더이상 증강이 남아있는 플레이어가 없습니다");
                break;
            }
            int playerNo = Random.Range(0, haveAugmentPlyerList.Count);
            Player curPlayer = haveAugmentPlyerList[playerNo];
            WeaponAugment data = curPlayer.GetAugment(); //정보를 하나 가져와보자.
            augmentPanel.AddData(data,curPlayer.Portrait,i); //여기서 알아서 i번째 UI에 UI를 넣도록 함.

            //이벤트 등록
            selectedAugmentList.Add(data);
            data.OnSelected += OnSelectedAugment;
            needShowPanel = true;
        }
        if(needShowPanel)
        {
            augmentPanel.Show();
        }
        


    }
    public void OnSelectedAugment(WeaponAugment augment)
    {
        //선택된거 1개 제외 모두 다시 반환하도록 한다.
        HashSet<Weapon> weaponNeedUptate = new HashSet<Weapon>();
        foreach(var item in selectedAugmentList)
        {
            if(item != augment)
            {
                //선택 안한 증강은 다시 넣어주자 
                item.weapon.ReturnAugment(item.dataSO);
            }
            weaponNeedUptate.Add(item.weapon);
        }
        //업데이트를 return 시켜줬기때문에 다시 다 Update 해줘야함.
        foreach(var weapon in weaponNeedUptate)
        {
            weapon.UpdateWeaponData();
        }

        ClearSelectAugmentList();
        augmentPanel.Close();
    }

    private void ClearSelectAugmentList()
    {
        foreach (var item in selectedAugmentList)
        {
            if (item != null)
            {
                item.OnSelected -= OnSelectedAugment;
            }
        }
        selectedAugmentList.Clear();
    }
}
