using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectManager : MonoBehaviour, IMainMenuPanel
{
    [SerializeField] WorldSelectButton[] worldSelectButtonArray;
    [SerializeField] StageSelectManager[] stageSelectManagerArray; //인덱스만큼 해당 패널이 열릴거임

    //캐릭터 선택 패널을 띄운다.
    [SerializeField] CharacterSelectPanel characterSelectPanel;
    private void Awake()
    {
        //유효성 체크
        HashSet<int> worldIndexHash = new HashSet<int>();
        foreach(WorldSelectButton curWorldButton in worldSelectButtonArray)
        {
            if(worldIndexHash.Contains(curWorldButton.WorldIndex))
            {
                MyLog.LogError($" {curWorldButton.name}의 {curWorldButton.WorldIndex}가 중복되었습니다. 확인해주세요");
                continue;
            }
            worldIndexHash.Add(curWorldButton.WorldIndex);
            curWorldButton.OnWorldSelectButton += ShowStagePanel;
        }
        //스테이지 이벤트 구독 
        foreach(StageSelectManager curStageManager in stageSelectManagerArray)
        {
            curStageManager.StageSelectReqest += ShowCharacterSelectPanel;
        }

    }
    private void OnDestroy()
    {
        //이벤트 해제
        foreach(WorldSelectButton curWorldButton in worldSelectButtonArray)
        {
            curWorldButton.OnWorldSelectButton -= ShowStagePanel;
        }
        foreach (StageSelectManager curStageManager in stageSelectManagerArray)
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
    private void ShowCharacterSelectPanel(StageDataSO data)
    {
        //스테이지에서 선택한 정보를 통해 캐릭터 선택창을 띄운다.
        characterSelectPanel.ShowPanelAndSetData(data);
    }

    public void ShowPanel(bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (!isShow) { return; }
    }
}
