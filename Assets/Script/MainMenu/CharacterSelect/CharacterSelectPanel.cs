using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour
{
    [SerializeField] StatIconAll statIconAll;
    [SerializeField] SkillIconAll skillIconAll;
    [SerializeField] TextMeshProUGUI selectedCharacterNameText;
    [SerializeField] TextMeshProUGUI selectedCharacterLevelText;

    //가지고있는 캐릭터가 표시되는 스코롤뷰
    [SerializeField] CharacterSelectScrollView characterSelectScrollView;
    //선택한 캐릭터를 표시하는 UI매니저
    [SerializeField] CharacterSelectedUIManager characterSelectedUIManager;
    [SerializeField] Button selectCompleteButton;
    [SerializeField] Button closeButton;

    StageDataSO stageDataSO;
    WorldStageInfo worldStageInfo;
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
    private void OnDisable()
    {
        OnCloseButtonClick();
    }

    private void Init()
    {
        //모든 선택된 상태들에 대해서 초기화 해주자
        selectCompleteButton.interactable = false;
    }
    private void OnCloseButtonClick()
    {
        Init();
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
        List<CharacterInstance> selectedInstanceList = characterSelectedUIManager.GetSelectedCharacter();
        CharacterManager.Instance.SetSelectedCharacter(selectedInstanceList);
        StageManager.Instance.SetStageData(stageDataSO,worldStageInfo);
        //씬 이동
        SceneManager.LoadScene(SceneController.InGame);


    }

    public void SelectCharacterAndShowInfo(CharacterSelectButton selectButton, CharacterInstance characterInstance)
    {
        //캐릭터 정보 표시
        CharacterStat characterTotalStat = characterInstance.GetCharacterTotalStat();
        statIconAll.SetIcon(characterTotalStat);
        skillIconAll.SetIcon(characterInstance.characterData);
        selectedCharacterNameText.text = characterInstance.characterData.CharacterName;
        selectedCharacterLevelText.text = characterInstance.characterDataDb.level.ToString();

        //선택한 리스트에 하나씩 추가를 해준다.
        if (characterSelectedUIManager.SetData(characterInstance, out bool selected))
        {
            //변했으면 패널에 변경점이 있음
            selectButton.UpdateCheckImage(selected);
        }

    }
    public void ShowPanelAndSetData(StageDataSO data, WorldStageInfo worldStageData )
    {

        stageDataSO = data;
        worldStageInfo = worldStageData;
        characterSelectScrollView.CharacterShow(this);
        this.gameObject.SetActive(this);
        
    }
}
