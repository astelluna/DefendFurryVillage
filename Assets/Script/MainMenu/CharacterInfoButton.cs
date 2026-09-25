using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoButton : MonoBehaviour
{
    //캐릭터에 대한 정보를 가지고있는 버튼. 
    private CharacterDataSO characterDataSO;

    Button characterButton;

    public event Action<CharacterDataSO> OnCharcterBtnClick;

    private void Awake()
    {
        characterButton = GetComponent<Button>();
        characterButton.onClick.AddListener(OnButtonClick);
    }

    public void SetCharacterData(CharacterDataSO data)
    {
        characterDataSO = data;
        characterButton.image.sprite = data.CharacterPortraitSprite;
    }
    
    private void OnButtonClick()
    {
        OnCharcterBtnClick?.Invoke(characterDataSO);
    }

    
}
