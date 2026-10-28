using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIPanel : MonoBehaviour
{
    [SerializeField] StageSelectButton[] stageSelectButtonArray;
    [SerializeField] StageInfoPanel stageInfoPanel;
    [SerializeField] Button closeButton;

    StageDataSO stageDataSO;
    WorldStageInfo stageInfo;

    public int WorldNum { get; private set; }
    public int StageButtonCount => stageSelectButtonArray.Length;
    public event Action<StageDataSO,WorldStageInfo> StageSelectReqest;
    private void Awake()
    {
        closeButton.onClick.AddListener(ClosePanel);
        stageInfoPanel.StageEnterRequest += SelectedStage;
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
    private void SelectedStage()
    {
        //스테이지가 선택되었으니 이 정보를 통해 캐릭터 선택창으로 진입 필요
        StageSelectReqest?.Invoke(stageDataSO, stageInfo);
        
    }
    private void OnDisable()
    {
        ClosePanel();
    }


    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        stageInfoPanel.StageEnterRequest -= SelectedStage;
        foreach (var item in stageSelectButtonArray)
        {
            item.OnStageButtonClicked -= ShowStageInfo;
        }
    }
    private void ShowStageInfo(StageDataSO data, WorldStageInfo worldStageInfo)
    {
        //해당 스테이지의 버튼을 누르면 해당 스테이지의 정보로 info를 표시하는 패널을 하나 더 띄운다. 
        stageDataSO = data;
        stageInfo = worldStageInfo;
        stageInfoPanel.ShowPanel(data);
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        //기록을 확인해서, 만약에 정보보다 적다면? 버튼을 비활성화
        StageProgressData bestRecord =  StageManager.Instance.BestStageProgressData;

        //현재 스테이지 월드가 최고기록 월드보다 작다면? 스테이지 버튼 다 비활성화
        if(WorldNum > bestRecord.worldId)
        {
            //버튼 전부 비활성화
            foreach(StageSelectButton item in stageSelectButtonArray)
            {
                item.ActiveButton(false);
            }
        }
        else
        {
            //for문 돌면서 인덱스위치 비교
            for(int i=0; i< stageSelectButtonArray.Length; i++)
            {
                int stageIndex = i + 1;
                if (WorldNum == bestRecord.worldId)
                {
                    if(stageIndex > bestRecord.stageId)
                    {
                        stageSelectButtonArray[i].ActiveButton(false);
                    }
                    else
                    {
                        stageSelectButtonArray[i].ActiveButton(true);
                    }
                }
                else
                {
                    stageSelectButtonArray[i].ActiveButton(true);
                }
            }
        }
    }
    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void SetData(WorldDataSO worldDataSO, int worldIdx)
    {
        if(worldDataSO.StageDataList.Count != stageSelectButtonArray.Length)
        {
            MyLog.LogError($"데이터는{worldDataSO.StageDataList.Count}개인데 UI는 {stageSelectButtonArray.Length}개 입니다. 동일하게 맞춰주세요");
        }
        //월드 인덱스 넣기 
        WorldNum = worldIdx;
        for (int i=0; i< stageSelectButtonArray.Length; i++)
        {
            if(i >= worldDataSO.StageDataList.Count)
            {
                continue;
            }
            StageDataSO curStageData = worldDataSO.StageDataList[i];
            int stageIdx = i + 1;
            stageSelectButtonArray[i].SetData(curStageData, worldIdx, stageIdx, stageSelectButtonArray.Length);
        }
        

        //아래 버튼들의 스테이지 인덱스를 생성해준다.
    }


}
