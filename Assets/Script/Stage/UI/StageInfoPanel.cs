using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageName;
    [SerializeField] TextMeshProUGUI stageDesc;
    [SerializeField] TextMeshProUGUI rewardGoldText;
    [SerializeField] Button closeButton;
    [SerializeField] Button stageEnterButton;
    //StageDataSO stageDataSO;
    List<StageInfoEnemyUI> stageInfoEnemyUIList = new List<StageInfoEnemyUI>();


    [SerializeField] StageInfoEnemyUI stageInfoEnemyUIPrefab;
    [SerializeField] Transform spawnPos;

    public event Action StageEnterRequest;

    //아이템 보상정보 표시하는 프리팹 객체
    [SerializeField] Transform rewardItemMakePosition;
    [SerializeField] RewardItemUI rewardItemUI;
    List<RewardItemUI> rewardItemUIList = new List<RewardItemUI>();

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        stageEnterButton.onClick.AddListener(OnStageEnterButtonClick);
    }
    private void OnDisable()
    {
        CloseButtonClick();
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        stageEnterButton.onClick.RemoveAllListeners();
    }
    private void OnStageEnterButtonClick()
    {
        StageEnterRequest?.Invoke();
    }
    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowPanel(StageDataSO data)
    {
        Init();
        //스테이지 정보 넣기
        stageName.text = data.StageName;
        stageDesc.text = data.StageDesc;
        rewardGoldText.text = data.RewardData.RewardMoney.ToString() + " G";

        //몬스터 종류가 어떤게 있는지 찾기 
        HashSet<EnemyDataSO> enemyDataHash = new HashSet<EnemyDataSO>();
        List<EnemyDataSO> enemyList = new List<EnemyDataSO>();
        foreach (WaveDataSO waveData in data.WaveDataList)
        {
            foreach (var enemyData in waveData.EnemieList)
            {
                if (!enemyDataHash.Contains(enemyData))
                {
                    enemyDataHash.Add(enemyData);
                    enemyList.Add(enemyData);
                }
            }
        }
        //몬스터 종류에 따라 UI에 표시
        if (enemyList.Count > stageInfoEnemyUIList.Count)
        {
            int needCount = enemyList.Count - stageInfoEnemyUIList.Count;
            for (int i = 0; i < needCount; i++)
            {
                StageInfoEnemyUI infoPrefab = Instantiate(stageInfoEnemyUIPrefab, spawnPos);
                stageInfoEnemyUIList.Add(infoPrefab);
            }
        }
        //for문 돌면서 정보 설정하고 뒤에것들은 꺼준다. 
        for (int i = 0; i < stageInfoEnemyUIList.Count; i++)
        {
            if (i >= enemyList.Count)
            {
                //표시될 몬스터보다 UI가 많으면 비활성화
                stageInfoEnemyUIList[i].gameObject.SetActive(false);
            }
            else
            {
                //몬스터 표시 
                stageInfoEnemyUIList[i].SetImage(enemyList[i].Portrite);

            }
        }
        //보상 정보도 표시해준다.
        //TODO. 추후 오브젝트풀로 전환. 일단은 매번 생성하자 
        RewardDataSO rewardDataSO = data.RewardData;
        foreach(RewardItem item in rewardDataSO.RewardItemList)
        {
            RewardItemUI rewardItemPrefab =  Instantiate(rewardItemUI, rewardItemMakePosition);
            rewardItemPrefab.SetData(item);
            rewardItemUIList.Add(rewardItemPrefab);
        }

        this.gameObject.SetActive(true);
    }
    private void Init()
    {
        //보상정보 리스트 만들어준거 파괴해준다
        foreach(var item in rewardItemUIList)
        {
            Destroy(item.gameObject);
        }
        rewardItemUIList.Clear();
    }


}