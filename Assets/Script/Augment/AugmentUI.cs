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
    Player ply;

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
        else if(ply != null)
        {
            ply.PlayerSpawnEventInvoke();
        }
    }

    public void SetData(WeaponAugment augment, Sprite charcterSprite)
    {
        weaponAugment = augment;
        characterImage.sprite = charcterSprite;
        iconImage.sprite = weaponAugment.weapon.WeaponIcon;
        descriptionText.text = weaponAugment.dataSO.GetDataDescription();
    }
    public void SetData(Player playerData)
    {
        ply = playerData;
        characterImage.sprite = ply.Portrait;
        iconImage.sprite = ply.GetSpecialWeaponIcon();
        descriptionText.text = ply.GetSpecialWeaponDesc();
    }
    public void Init()
    {
        weaponAugment = null;
        ply = null;
    }
}
