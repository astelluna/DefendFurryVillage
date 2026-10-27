using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    //캐릭터 선택창에 있는 캐릭터 버튼들. 선택하면 이벤트 발동, 
    [SerializeField] Image checkImage;
    Button button;
    CharacterInstance characterInstanceData;

    //public event Action<CharacterSelectButton ,CharacterDataSO> OnCharacterSelectButtonClicked;
    public event Action<CharacterSelectButton, CharacterInstance> OnCharacterSelectButtonClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCharacterButtonClick);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetCharacterSelectButton(CharacterInstance characterInstance)
    {
        characterInstanceData = characterInstance;
        button.image.sprite = characterInstanceData.characterData.CharacterPortraitSprite;
    }
    

    private void OnCharacterButtonClick()
    {
        //한번 누르면 추가, 다시 누르면 취소를 해야함.

        OnCharacterSelectButtonClicked?.Invoke(this, characterInstanceData);
        //UpdateCheckImage();
    }

    public void UpdateCheckImage(bool isSelected)
    {
        checkImage.gameObject.SetActive(isSelected);
    }
    public void Init()
    {
        characterInstanceData = null;
        button.image.sprite = null;
        checkImage.gameObject.SetActive(false);
    }


}
