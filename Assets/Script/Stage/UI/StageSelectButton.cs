using System;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    //매니저를 만들어서.스테이지에 동일한 id값이 존재하는지 체크해야함. 그리고 버튼들을 매니저에 다 미리 등록을 해둘거임. or GetComponent로 하던가.;

    [field: SerializeField] public StageDataSO stageDataSO { get; private set; }
    [SerializeField] Button stageButton;
    public event Action<StageDataSO> OnStageButtonClicked;

    private void Awake()
    {
        stageButton.onClick.AddListener(StageButtonClick);
    }
    private void OnDestroy()
    {
        stageButton.onClick.RemoveAllListeners();
    }

    private void StageButtonClick()
    {
        OnStageButtonClicked?.Invoke(stageDataSO);
    }


}
