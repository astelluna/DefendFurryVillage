using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
            if(CaracterSelectButtonSpawner.Instance != null)
            {
                CaracterSelectButtonSpawner.Instance.ReturnPrefabToPool(item);
            }
        }
        characterSelectButtonList.Clear();
    }

    public void CharacterShow(CharacterSelectPanel selectPanel)
    {
        characterSelectPanel = selectPanel;

        //캐릭터를 가져오는데, 해금되지 않은 캐릭터는 불러오지 말아야함. (아예 불러오지말자)
        //정보를 가져오기. (현재 플레이어가 가지고있는 캐릭터)

        //TEST -> 나중에 HaveCharacter로 바꾸기
        //IReadOnlyList<CharacterDataSO> characterDataSOList = CharacterManager.Instance.HaveCharacterSOList;
        
        IReadOnlyDictionary<string, CharacterInstance> haveCharacterDict = CharacterManager.Instance.HaveCharacterDict;
        foreach(KeyValuePair<string, CharacterInstance> pair in haveCharacterDict)
        {
            string characterId = pair.Key;
            CharacterSelectButton buttonPrefab = CaracterSelectButtonSpawner.Instance.GetPrefabByPool(makePosition);
            buttonPrefab.SetCharacterSelectButton(pair.Value);
            buttonPrefab.OnCharacterSelectButtonClicked += OnCharacterSelect;
            characterSelectButtonList.Add(buttonPrefab);
        }
    }

    private void OnCharacterSelect(CharacterSelectButton selectButton, CharacterInstance characterInstance)
    {
        
        characterSelectPanel.SelectCharacterAndShowInfo(selectButton, characterInstance);
    }

}
