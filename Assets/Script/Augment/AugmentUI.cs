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
        weaponAugment.SelectedEventInvoke(weaponAugment);
    }

    public void SetData(WeaponAugment augment, Sprite charcterSprite)
    {
        weaponAugment = augment;
        characterImage.sprite = charcterSprite;
        iconImage.sprite = weaponAugment.weapon.WeaponIcon;
        descriptionText.text = weaponAugment.dataSO.GetDataDescription();
    }
}
