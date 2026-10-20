using System.Collections.Generic;
using UnityEngine;

public class CharacterShowPanel : MonoBehaviour, IMainMenuPanel
{
    //패널을 열면 모든 캐릭터에 대해서 버튼으로 프리팹을 하나 생성해준다. 
    //[SerializeField] CharacterInfoButton characterButtonPrefab;
    [SerializeField] Transform haveCharacterMakePosition; //보유 캐릭터 생성 위치 - 스코롤뷰 Content 위치
    [SerializeField] Transform unHaveCharacterMakePosition; //미보유 캐릭터 생성 위치

    [SerializeField] CharacterInfoPanel characterInfoPanel;

    List<CharacterInfoButton> characterinfoButtonList = new List<CharacterInfoButton>();

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
        foreach (var item in characterinfoButtonList)
        {
            item.OnCharcterBtnClick -= OnShowCharacterInfoPanel;
            CharacterInfoButtonSpawner.Instance.ReturnPrefabToPool(item);
        }
        characterinfoButtonList.Clear();
    }

    private void MakeAndShowCharacterButton()
    {
        //정보를 가져오기. 
        IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.AllCharacterSOList;

        foreach (CharacterDataSO item in characterDataSOList)
        {
            //보유중인 캐릭터, 아닌지에 따라 UI가 생성되는 위치가 달라짐
            Transform makePosition = null;
            if(CharacterManager.Instance.HaveCharacter(item.CharacterId))
            {
                makePosition = haveCharacterMakePosition;
            }
            else
            {
                makePosition = unHaveCharacterMakePosition;
            }
            CharacterInfoButton buttonPrefab = CharacterInfoButtonSpawner.Instance.GetPrefabByPool(makePosition);
            buttonPrefab.SetCharacterData(item);
            buttonPrefab.OnCharcterBtnClick += OnShowCharacterInfoPanel;
            characterinfoButtonList.Add(buttonPrefab);
        }
    }

    private void OnShowCharacterInfoPanel(CharacterDataSO data)
    {
        characterInfoPanel.SetAndShowPanel(data);
    }
    public void ShowPanel(bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (!isShow) { return; }

        MakeAndShowCharacterButton();

    }

}
