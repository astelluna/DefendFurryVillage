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
        //정보를 가져오기. (현재 플레이어가 가지고있는 캐릭터)
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
