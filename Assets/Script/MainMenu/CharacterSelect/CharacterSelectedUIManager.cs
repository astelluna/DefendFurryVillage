using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectedUIManager : MonoBehaviour
{
    [SerializeField] List<CharacterSelectedShowUI> selectedUIList = new List<CharacterSelectedShowUI>();
    Dictionary<CharacterDataSO, CharacterSelectedShowUI> selectedCharacterDict = new Dictionary<CharacterDataSO, CharacterSelectedShowUI>();
    private bool selectedState = false;
    public bool HaveSelectedCharacter => selectedCharacterDict.Count > 0;

    public event Action<bool> OnChangeSelectedState; //캐릭터가 선택/비선택 되었을때 외부에 게임시작 버튼을 활성/비활성화 할거임

    public void CheckState()
    {
        bool state = HaveSelectedCharacter;
        if (state != selectedState)
        {
            selectedState = state;
            OnChangeSelectedState?.Invoke(selectedState);
        }
    }

    public bool FindEmptyIndex(out int num)
    {
        //UI중에서 빈 공간을 찾고 반환한다.
        for (int i = 0; i< selectedUIList.Count; i++)
        {
            if (!selectedUIList[i].gameObject.activeInHierarchy)
            {
                num = i;
                return true;
            }
        }
        num = int.MinValue;
        return false;
    }
    public bool SetData(CharacterDataSO data, out bool isSelected)
    {
        //1. 딕셔너리 만들어서 확인
        if(selectedCharacterDict.ContainsKey(data))
        {
            CharacterSelectedShowUI selectedUI = selectedCharacterDict[data];
            selectedUI.CancelUI();
            selectedCharacterDict.Remove(data);
            isSelected = false;
            CheckState();
            return true;
        }
        //빈자리가 없다면 return;
        if (!FindEmptyIndex(out int index))
        {
            isSelected = false;
            return false;
        }
        selectedUIList[index].SetUI(data);
        selectedCharacterDict.Add(data, selectedUIList[index]);
        isSelected = true;
        CheckState();
        return true;
    }

    

    public List<CharacterDataSO> GetSelectedCharacter()
    {
        //게임시작 버튼을 누르면. 활성화되어있는 UI버튼이 선택된 것들이므로 확인해서 가져온다.
        List<CharacterDataSO> characterList = new List<CharacterDataSO>();
        foreach(CharacterSelectedShowUI item in selectedUIList)
        {
            if(item.gameObject.activeInHierarchy)
            {
                characterList.Add(item.CharacterSO);
            }
        }
        return characterList;
    }


}
