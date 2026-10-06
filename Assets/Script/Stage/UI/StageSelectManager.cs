using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] StageSelectButton[] stageSelectButtonArray;
    [SerializeField] StageInfoPanel stageInfoPanel;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(ClosePanel);
        //스테이지 버튼들에 체크. 중복된것들이 있는지 확인한다
        HashSet<StageDataSO> stageDataSOHash = new HashSet<StageDataSO>();
        foreach(var item in stageSelectButtonArray)
        {
            if(item.stageDataSO == null)
            {
                MyLog.LogError($" {item.name}에 스테이지 SO가 없음");
                continue;
            }
            if(stageDataSOHash.Contains(item.stageDataSO))
            {
                MyLog.LogError($" {item.name}에 있는 {item.stageDataSO.name} SO데이터가 중복됨.");
                continue;
            }
            item.OnStageButtonClicked += ShowStageInfo;
        }
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        foreach (var item in stageSelectButtonArray)
        {
            item.OnStageButtonClicked -= ShowStageInfo;
        }
    }
    private void ShowStageInfo(StageDataSO data)
    {
        //해당 스테이지의 버튼을 누르면 해당 스테이지의 정보로 info를 표시하는 패널을 하나 더 띄운다. 
        stageInfoPanel.ShowPanel(data);
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }



}
