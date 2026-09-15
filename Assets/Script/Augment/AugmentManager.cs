using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }

    [SerializeField] AugmentPanel augmentPanel;

    List<WeaponAugment> selectedAugmentList = new List<WeaponAugment>();
    List<Player> haveAugmentPlyerList = new List<Player>();

    private bool isShowingPanel = false; //현재 패널 띄워져있는지 플래그

    int maxShowCount;
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
        //Invoke( nameof(ShowAugmentUI), 3.0f);
        //ShowAugmentUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        if (isShowingPanel) Time.timeScale = 1f;
        ClearSelectAugmentList();
    }

    public void SetAndShowAugmentUI(int cnt)
    {
        //maxShowCount = cnt;
        //if (curShowCount >= maxShowCount) return;
        maxShowCount += cnt;
        if (maxShowCount <= 0) return;
        if (isShowingPanel) return;
        Time.timeScale = 0f;
        ShowAugmentUI();
    }

    private void ShowAugmentUI()
    {

        isShowingPanel = true;
        maxShowCount--;
        //UI를 보여주고 현재 플레이어중 1개를 선택해서 쭉쭉 내려간다음에 증강 1개를 선택해서 가져온다.
        bool needShowPanel = false;
        ClearSelectAugmentList();
        
        for(int i=0; i< 3; i++)
        {
            haveAugmentPlyerList = PlayerManager.Instance.GetHaveAugmentPlayerList();
            if (haveAugmentPlyerList.Count == 0)
            {
                if(i==0)
                {
                    //단 1번이라도 실행이 안되었었다면? 시간 다시 흘러가야함
                    Time.timeScale = 1f;
                    isShowingPanel = false;
                    maxShowCount = 0;
                }
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
        MyLog.Log("이벤트 발동");
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
        if(maxShowCount <= 0)
        {
            Time.timeScale = 1f;
            isShowingPanel = false;
            return;
        }
        else
        {
            //연속 레벨업을 체크해야하므로 추가로 계속 해준다.
            ShowAugmentUI();
        }
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
