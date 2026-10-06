using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectManager : MonoBehaviour
{
    [SerializeField] WorldSelectButton[] worldSelectButtonArray;
    [SerializeField] StageSelectManager[] stageSelectManagerArray; //인덱스만큼 해당 패널이 열릴거임
    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        //유효성 체크
        HashSet<int> worldIndexHash = new HashSet<int>();
        foreach(var item in worldSelectButtonArray)
        {
            if(worldIndexHash.Contains(item.WorldIndex))
            {
                MyLog.LogError($" {item.name}의 {item.WorldIndex}가 중복되었습니다. 확인해주세요");
                continue;
            }
            worldIndexHash.Add(item.WorldIndex);
            item.OnWorldSelectButton += ShowStagePanel;
        }

    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        foreach(var item in worldSelectButtonArray)
        {
            item.OnWorldSelectButton -= ShowStagePanel;
        }
    }

    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
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
}
