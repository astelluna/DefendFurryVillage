using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUpgradePanel : MonoBehaviour
{
    [SerializeField] Image equipmentImage;

    //업그레이드로 증가하는 량 표시
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] TextMeshProUGUI criticalPercentText;
    [SerializeField] TextMeshProUGUI criticalRatioText;
    [SerializeField] TextMeshProUGUI armorPenetrateText;

    [SerializeField] TextMeshProUGUI infoText; 
    [SerializeField] Button upgradeButton;
    [SerializeField] Button closeButton;

    EquipmentInstance equipmentInstance;
    EquipmentInfoPanel equipmentInfoPanel;
    const int maxLevel = 5;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    private void OnDisable()
    {
        OnCloseButtonClicked();
    }
    private void OnDestroy()
    {
        upgradeButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnCloseButtonClicked()
    {
        this.gameObject.SetActive(false);
    }
    private async void OnUpgradeButtonClicked()
    {
        LoadingPanelMainScene.Instance.ShowPanel(true);
        string equipmentId = equipmentInstance.EquipmentDataSO.Id;
        await FirestoreManager.Instance.UpgradeItemAsync(eItemtype.equipment, equipmentId, equipmentInstance.Level, equipmentId, equipmentInstance.Level + 1);
        LoadingPanelMainScene.Instance.ShowPanel(false);
        //전부 끝나면 닫음
        equipmentInfoPanel.OnRequestUpdate();
        OnCloseButtonClicked();
    }

    public void ShowPanel(EquipmentInstance equipmentInfo, EquipmentInfoPanel infoPanel)
    {
        equipmentInstance = equipmentInfo;
        equipmentInfoPanel = infoPanel;
        equipmentImage.sprite = equipmentInstance.EquipmentDataSO.SpriteImage;


        if(equipmentInfo.Level == maxLevel)
        {
            attackText.text = "0";
            criticalPercentText.text = "0";
            criticalRatioText.text = "0";
            armorPenetrateText.text = "0";
            infoText.text = "최대 레벨로 강화가 불가능합니다";
            upgradeButton.interactable = false;
        }
        else
        {
            attackText.text = equipmentInstance.EquipmentDataSO.UpgradeAttackValue.ToString();
            criticalPercentText.text = equipmentInstance.EquipmentDataSO.UpgradeCriticalRate.ToString();
            criticalRatioText.text = equipmentInstance.EquipmentDataSO.UpgradeCriticalRatio.ToString();
            armorPenetrateText.text = equipmentInstance.EquipmentDataSO.UpgradeArmorPenetrate.ToString();
            infoText.text = "강화버튼을 누르면 강화를 진행합니다";
            upgradeButton.interactable = true;
        }
        this.gameObject.SetActive(true);
    }
}
