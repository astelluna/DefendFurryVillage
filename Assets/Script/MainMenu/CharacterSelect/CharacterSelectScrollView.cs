using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectScrollView : MonoBehaviour
{
    [SerializeField] CharacterSelectButton characterSelectButton;
    [SerializeField] Transform makePosition;
    CharacterSelectPanel characterSelectPanel;

    List<CharacterSelectButton> characterSelectButtonList = new List<CharacterSelectButton>();

    private void OnDestroy()
    {
        foreach(CharacterSelectButton item in characterSelectButtonList)
        {
            item.OnCharacterSelectButtonClicked -= OnCharacterSelect;
        }
    }

    public void CharacterShow(CharacterSelectPanel selectPanel)
    {
        characterSelectPanel = selectPanel;

        //캐릭터를 가져오는데, 해금되지 않은 캐릭터는 불러오지 말아야함. (아예 불러오지말자)
        //정보를 가져오기. (현재 플레이어가 가지고있는 캐릭터)
        IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.HaveCharacterSOList;
        foreach (CharacterDataSO item in characterDataSOList)
        {
            //TODO. 추후에 캐릭터 많아지면 이거도 오브젝트풀 해두기
            CharacterSelectButton buttonPrefab = Instantiate(characterSelectButton, makePosition);
            buttonPrefab.SetCharacterSelectButton(item);
            buttonPrefab.OnCharacterSelectButtonClicked += OnCharacterSelect;
            characterSelectButtonList.Add(buttonPrefab);
        }
    }
    private void OnCharacterSelect(CharacterSelectButton selectButton , CharacterDataSO characterDataSO)
    {
        characterSelectPanel.SelectCharacterAndShowInfo(selectButton, characterDataSO);
    }

}
