using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageName;
    [SerializeField] TextMeshProUGUI stageDesc;
    [SerializeField] Button closeButton;
    StageDataSO stageDataSO;
    List<StageInfoEnemyUI> stageInfoEnemyUIList = new List<StageInfoEnemyUI>();


    [SerializeField] StageInfoEnemyUI stageInfoEnemyUIPrefab;
    [SerializeField] Transform spawnPos;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
    }
    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowPanel(StageDataSO data)
    {
        //스테이지 정보 넣기
        stageDataSO = data;
        stageName.text = stageDataSO.StageName;
        stageDesc.text = stageDataSO.StageDesc;

        //몬스터 종류가 어떤게 있는지 찾기 
        HashSet<EnemyDataSO> enemyDataHash = new HashSet<EnemyDataSO>();
        List<EnemyDataSO> enemyList = new List<EnemyDataSO>();
        foreach (WaveDataSO waveData in stageDataSO.WaveDataList)
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

        this.gameObject.SetActive(true);
    }


}