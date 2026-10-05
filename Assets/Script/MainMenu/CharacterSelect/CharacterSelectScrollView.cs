using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectScrollView : MonoBehaviour
{
    
    [SerializeField] Transform makePosition;
    CharacterSelectPanel characterSelectPanel;

    List<CharacterSelectButton> characterSelectButtonList = new List<CharacterSelectButton>();

    private void OnDestroy()
    {
        Init();
    }

    private void OnDisable()
    {
        Init();
    }

    private void Init()
    {
        foreach (CharacterSelectButton item in characterSelectButtonList)
        {
            item.OnCharacterSelectButtonClicked -= OnCharacterSelect;
            CaracterSelectButtonSpawner.Instance.ReturnPrefabToPool(item);
        }
        characterSelectButtonList.Clear();
    }

    public void CharacterShow(CharacterSelectPanel selectPanel)
    {
        //TODO. 오브젝트풀로 버튼들 바꾼다음에 매번 열때마다 정보 바꿔야함
        characterSelectPanel = selectPanel;

        //캐릭터를 가져오는데, 해금되지 않은 캐릭터는 불러오지 말아야함. (아예 불러오지말자)
        //정보를 가져오기. (현재 플레이어가 가지고있는 캐릭터)

        //TEST -> 나중에 HaveCharacter로 바꾸기
        //IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.HaveCharacterSOList;
        IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.AllCharacterSOList;

        foreach (CharacterDataSO item in characterDataSOList)
        {
            //TODO. 추후에 캐릭터 많아지면 이거도 오브젝트풀 해두기
            CharacterSelectButton buttonPrefab = CaracterSelectButtonSpawner.Instance.GetPrefabByPool(makePosition);
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
