using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectedShowUI : MonoBehaviour
{
    [SerializeField] Button button;
    public CharacterDataSO CharacterSO { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSelectedButtonClick);
    }

    public void SetUI(CharacterDataSO data)
    {
        CharacterSO = data;
        button.image.sprite = CharacterSO.CharacterPortraitSprite;
        this.gameObject.SetActive(true);
    }

    public void OnSelectedButtonClick()
    {
        //CancelUI();
        //그냥 캐릭터 이름이 뜨도록 할까? 일단 보류
    }
    public void CancelUI()
    {
        CharacterSO = null;
        this.gameObject.SetActive(false);
    }

}
