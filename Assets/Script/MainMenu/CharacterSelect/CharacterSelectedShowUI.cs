using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectedShowUI : MonoBehaviour
{
    [SerializeField] Button button;
    public CharacterInstance CharacterInstanceData { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSelectedButtonClick);
    }

    public void SetUI(CharacterInstance data)
    {
        CharacterInstanceData = data;
        button.image.sprite = CharacterInstanceData.characterData.CharacterPortraitSprite;
        this.gameObject.SetActive(true);
    }

    public void OnSelectedButtonClick()
    {
        //CancelUI();
        //그냥 캐릭터 이름이 뜨도록 할까? 일단 보류
    }
    public void CancelUI()
    {
        CharacterInstanceData = null;
        this.gameObject.SetActive(false);
    }

}
