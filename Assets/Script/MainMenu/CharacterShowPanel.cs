using System.Collections.Generic;
using UnityEngine;

public class CharacterShowPanel : MonoBehaviour
{
    //패널을 열면 모든 캐릭터에 대해서 버튼으로 프리팹을 하나 생성해준다. 
    [SerializeField] CharacterInfoButton characterButtonPrefab;
    [SerializeField] Transform makePosition; //스코롤뷰 Content 위치
    [SerializeField] CharacterInfoPanel characterInfoPanel;

    List<CharacterInfoButton> characterinfoButtonList = new List<CharacterInfoButton>();


    private void Start()
    {
        //테스트용 딜레이 시작
        Invoke(nameof(DelayStart), 10f);
    }
    private void OnDestroy()
    {
        foreach(var item in characterinfoButtonList)
        {
            item.OnCharcterBtnClick -= OnShowCharacterInfoPanel;
        }
        characterinfoButtonList.Clear();
    }

    private void DelayStart()
    {
        //정보를 가져오기. 
        IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.AllCharacterSOList;

        foreach (CharacterDataSO item in characterDataSOList)
        {
            CharacterInfoButton buttonPrefab = Instantiate(characterButtonPrefab, makePosition);
            buttonPrefab.SetCharacterData(item);
            buttonPrefab.OnCharcterBtnClick += OnShowCharacterInfoPanel;
            characterinfoButtonList.Add(buttonPrefab);
        }
    }

    private void OnShowCharacterInfoPanel(CharacterDataSO data)
    {
        characterInfoPanel.SetAndShowPanel(data);
    }

}
