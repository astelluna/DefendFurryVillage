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
    //CharacterDataSO characterSO;
    Character characterPrefab;

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
        else if(characterPrefab != null)
        {
            characterPrefab.CharacterSpawnEventInvoke();
        }
    }

    public void SetData(WeaponAugment augment, Sprite charcterSprite)
    {
        weaponAugment = augment;
        characterImage.sprite = charcterSprite;
        iconImage.sprite = weaponAugment.weapon.WeaponDataSO.WeaponIcon;
        descriptionText.text = weaponAugment.dataSO.GetDataDescription();
    }
    public void SetData(Character character)
    {
        characterPrefab = character;
        characterImage.sprite = characterPrefab.CharacterDataSO.CharacterPortraitSprite;
        iconImage.sprite = characterPrefab.CharacterDataSO.SpecialWeaponDataSO.WeaponIcon;
        descriptionText.text = characterPrefab.CharacterDataSO.SpecialWeaponDataSO.WeaponDescription;

    }
    public void Init()
    {
        weaponAugment = null;
        characterPrefab = null;
    }
}
