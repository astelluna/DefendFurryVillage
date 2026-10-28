using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectManager : MonoBehaviour, IMainMenuPanel
{
    [SerializeField] WorldSelectButton[] worldSelectButtonArray;
    [SerializeField] WorldUIPanel[] stageSelectManagerArray; //인덱스만큼 해당 패널이 열릴거임

    //캐릭터 선택 패널을 띄운다.
    [SerializeField] CharacterSelectPanel characterSelectPanel;
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        //스테이지 이벤트 구독 
        foreach (WorldSelectButton curWorldButton in worldSelectButtonArray)
        {
            curWorldButton.OnWorldSelectButton += ShowStagePanel;
        }
        foreach (WorldUIPanel stagePanelMainUI in stageSelectManagerArray)
        {
            stagePanelMainUI.StageSelectReqest += ShowCharacterSelectPanel;
        }
        //UI 데이터를 넣는다 (런타임에)
        SetUIData();
    }
    private void SetUIData()
    {
        //UI데이터 넣기전에 유효성 검사부터 다 해준다
        //버튼과 실제 데이터가 유효한지 체크한다. -> 실패해도 진행은 할거임
        IReadOnlyList<WorldDataSO> worldData_SO = StageManager.Instance.AllWorldDataSoList;
        //월드 선택하는 버튼 갯수가 정상적인지 확인
        if (worldSelectButtonArray.Length != worldData_SO.Count)
        {
            Debug.LogError($"월드 데이터는{worldData_SO.Count}개인데, UI는 {worldSelectButtonArray.Length}개 입니다. 매칭이 안되어있음");
        }
        //월드선택하면 나오는 패널의 갯수가 정상적으로 존재하는지 확인
        if (stageSelectManagerArray.Length != worldData_SO.Count)
        {
            Debug.LogError($"월드 데이터는{worldData_SO.Count}개인데, 스테이지 패널UI는 {stageSelectManagerArray.Length}개 입니다. 매칭이 안되어있음");
        }
        //월드 패널 안에 스테이지버튼UI 갯수가 정상적인지 확인
        if (stageSelectManagerArray.Length == worldData_SO.Count)
        {
            for (int i = 0; i < worldData_SO.Count; i++)
            {
                if (stageSelectManagerArray[i].StageButtonCount != worldData_SO[i].StageDataList.Count)
                {
                    Debug.LogError($"{i + 1}번째 월드의 스테이지 데이터는{worldData_SO.Count}개인데, 스테이지 UI버튼 갯수는 {stageSelectManagerArray[i].StageButtonCount}개 입니다. 매칭이 안되어있음");
                }
            }
        }

        //버튼에 갯수만큼 인덱스를 넣어준다. 
        for (int i=0; i< worldSelectButtonArray.Length; i++)
        {
            string worldName = worldData_SO[i].worldName;
            worldSelectButtonArray[i].SetData(worldName, i + 1);
        }

        //데이터를 읽어서 UI에 설정
        for(int i=0; i< stageSelectManagerArray.Length; i++)
        {
            if(i >= worldData_SO.Count)
            {
                continue;
            }
            WorldDataSO worldData = worldData_SO[i];
            int worldIdx = i + 1;
            WorldUIPanel stagePanelMainUI = stageSelectManagerArray[i];
            stagePanelMainUI.SetData(worldData, worldIdx);
        }

    }
    private void OnDestroy()
    {
        //이벤트 해제
        foreach(WorldSelectButton curWorldButton in worldSelectButtonArray)
        {
            curWorldButton.OnWorldSelectButton -= ShowStagePanel;
        }
        foreach (WorldUIPanel curStageManager in stageSelectManagerArray)
        {
            curStageManager.StageSelectReqest -= ShowCharacterSelectPanel;
        }

    }

    private void ShowStagePanel(int num)
    {
        int panelIndex = num - 1; //실제 사용하는 인덱스

        if(panelIndex >= stageSelectManagerArray.Length)
        {
            MyLog.LogError($"월드 인덱스가 너무 높습니다. {stageSelectManagerArray.Length}개인데 {num}이 들어옴 ");
            return;
        }

        stageSelectManagerArray[panelIndex].ShowPanel();
    }
    private void ShowCharacterSelectPanel(StageDataSO data, WorldStageInfo worldStageInfo)
    {
        //스테이지에서 선택한 정보를 통해 캐릭터 선택창을 띄운다.
        characterSelectPanel.ShowPanelAndSetData(data, worldStageInfo);
    }

    public void ShowPanel(bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (!isShow) { return; }
    }
}
