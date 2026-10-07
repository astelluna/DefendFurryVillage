using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour
{
    [SerializeField] StatIconAll statIconAll;
    [SerializeField] SkillIconAll skillIconAll;

    //가지고있는 캐릭터가 표시되는 스코롤뷰
    [SerializeField] CharacterSelectScrollView characterSelectScrollView;
    //선택한 캐릭터를 표시하는 UI매니저
    [SerializeField] CharacterSelectedUIManager characterSelectedUIManager;
    [SerializeField] Button selectCompleteButton;
    [SerializeField] Button closeButton;

    StageDataSO stageDataSO;

    private void Awake()
    {
        characterSelectedUIManager.OnChangeSelectedState += OnChangeGameStartButtonState;
        selectCompleteButton.onClick.AddListener(OnSelectedCompleteButton);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    private void OnDestroy()
    {
        characterSelectedUIManager.OnChangeSelectedState -= OnChangeGameStartButtonState;
        selectCompleteButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    private void OnChangeGameStartButtonState(bool isInteractable)
    {
        selectCompleteButton.interactable = isInteractable;
    }

    private void OnSelectedCompleteButton()
    {
        //선택된 캐릭터가 하나라도 없다면 안함
        if(!characterSelectedUIManager.HaveSelectedCharacter)  
        {
            MyLog.LogError("애초에 버튼이 비활성화 되었어야하는데 뭔가 이상함.");
            return; 
        }
        //다음 인게임씬으로 이동하기전에 싱글톤 매니저들에 정보를 설정해준다 
        List<CharacterDataSO> selectedCharacterDataSO = characterSelectedUIManager.GetSelectedCharacter();
        CharacterManager.Instance.SetSelectedCharacter(selectedCharacterDataSO);
        StageManager.Instance.SetStageData(stageDataSO);
        //씬 이동
        SceneManager.LoadScene(SceneController.InGame);

    }

    public void SelectCharacterAndShowInfo(CharacterSelectButton selectButton, CharacterDataSO characterDataSO)
    {
        //캐릭터 정보 표시
        statIconAll.SetIcon(characterDataSO);
        skillIconAll.SetIcon(characterDataSO);

        //선택한 리스트에 하나씩 추가를 해준다.
        if(characterSelectedUIManager.SetData(characterDataSO, out bool selected))
        {
            //변했으면 패널에 변경점이 있음
            selectButton.UpdateCheckImage(selected);
        }

    }
    public void ShowPanelAndSetData(StageDataSO data)
    {

        stageDataSO = data;
        characterSelectScrollView.CharacterShow(this);
        this.gameObject.SetActive(this);
        
    }
}
