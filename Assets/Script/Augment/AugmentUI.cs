using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentUI : MonoBehaviour
{
    [SerializeField] Image characterImage;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button augmentButton;

    WeaponAugment weaponAugment;
    CharacterDataSO characterSO;

    private void Awake()
    {
        augmentButton.onClick.AddListener(() => AugmentButtonClick());
    }
    private void OnDestroy()
    {
        augmentButton.onClick.RemoveAllListeners();
    }
    private void AugmentButtonClick()
    {
        MyLog.Log("버튼 클릭");
        if(weaponAugment != null)
        {
            weaponAugment.SelectedEventInvoke(weaponAugment);
        }
        else if(characterSO != null)
        {
            characterSO.CharacterPrefab.CharacterSpawnEventInvoke();
        }
    }

    public void SetData(WeaponAugment augment, Sprite charcterSprite)
    {
        weaponAugment = augment;
        characterImage.sprite = charcterSprite;
        iconImage.sprite = weaponAugment.weapon.weaponDataSO.WeaponIcon;
        descriptionText.text = weaponAugment.dataSO.GetDataDescription();
    }
    public void SetData(CharacterDataSO characterData)
    {
        characterSO = characterData;
        characterImage.sprite = characterSO.CharacterPortraitSprite;
        iconImage.sprite = characterSO.CharacterPrefab.GetSpecialWeaponIcon();
        descriptionText.text = characterSO.CharacterPrefab.GetSpecialWeaponDesc();
    }
    public void Init()
    {
        weaponAugment = null;
        characterSO = null;
    }
}
