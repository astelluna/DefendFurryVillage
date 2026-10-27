using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectedUIManager : MonoBehaviour
{
    //선택된 캐릭터들에 대해 표시되고, 게임시작 버튼 누르면 해당 캐릭터 정보를 보내는 클래스
    [SerializeField] List<CharacterSelectedShowUI> selectedUIList = new List<CharacterSelectedShowUI>();
    Dictionary<string, CharacterSelectedShowUI> selectedCharacterDict = new Dictionary<string, CharacterSelectedShowUI>();
    private bool selectedState = false;
    public bool HaveSelectedCharacter => selectedCharacterDict.Count > 0;

    public event Action<bool> OnChangeSelectedState; //캐릭터가 선택/비선택 되었을때 외부에 게임시작 버튼을 활성/비활성화 할거임

    private void OnDisable()
    {
        //비활성화 되면 데이터 초기화 해야함
        Init();
    }

    private void Init()
    {
        selectedState = false;
        foreach(var item in selectedUIList)
        {
            item.CancelUI();
        }
        selectedCharacterDict.Clear();
    }
    
    public void CheckState()
    {
        //게임시작 버튼 비활성/활성화를 위한 캐릭터를 1개라도 선택했는지 상태체크하는 메서드
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
    //public bool SetData(CharacterDataSO data, out bool isSelected)
    public bool SetData(CharacterInstance data, out bool isSelected)
    {
        string characterId = data.characterData.CharacterId;
        //1. 딕셔너리 만들어서 확인
        if(selectedCharacterDict.ContainsKey(characterId))
        {
            //이미 선택되었는데 다시 클릭했으면 캐릭터 선택 해제
            CharacterSelectedShowUI selectedUI = selectedCharacterDict[characterId];
            selectedUI.CancelUI();
            selectedCharacterDict.Remove(characterId);
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
        selectedCharacterDict.Add(characterId, selectedUIList[index]);
        isSelected = true;
        CheckState();
        return true;
    }

    

    public List<CharacterInstance> GetSelectedCharacter()
    {
        //게임시작 버튼을 누르면. 활성화되어있는 UI버튼이 선택된 것들이므로 확인해서 가져온다.
        
        List<CharacterInstance> characterInstanceList = new List<CharacterInstance>();
        foreach(KeyValuePair<string, CharacterSelectedShowUI> pair in selectedCharacterDict)
        {
            characterInstanceList.Add(pair.Value.CharacterInstanceData);
        }
        return characterInstanceList;
    }


}
