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
    [SerializeField] TextMeshProUGUI upgradeCostText;

    [SerializeField] TextMeshProUGUI infoText; 
    [SerializeField] Button upgradeButton;
    [SerializeField] Button closeButton;

    EquipmentInstance equipmentInstance;
    EquipmentInfoPanel equipmentInfoPanel;
    const int maxLevel = 5;
    private int cost;

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
        await FirestoreManager.Instance.UpgradeItemAsync(eItemtype.equipment, equipmentId, equipmentInstance.Level, equipmentId, equipmentInstance.Level + 1, cost);
        LoadingPanelMainScene.Instance.ShowPanel(false);
        //전부 끝나면 닫음
        equipmentInfoPanel.OnRequestUpdate();


        UpdatePanel();
        OnCloseButtonClicked();
    }

    private void UpdatePanel()
    {
        bool isMaxLevel = (equipmentInstance.Level == maxLevel);
        bool isEnoughMoney;

        //강화 비용 설정
        cost = (equipmentInstance.Level + 1) * equipmentInstance.EquipmentDataSO.UpgradeCost;
        upgradeCostText.text = cost.ToString();

        //장비 강화 버튼을 누를때마다 정보가 변경될거임
        if (isMaxLevel)
        {
            attackText.text = "0";
            criticalPercentText.text = "0";
            criticalRatioText.text = "0";
            armorPenetrateText.text = "0";
            infoText.text = "최대 레벨로 강화가 불가능합니다";
        }
        else
        {
            attackText.text = equipmentInstance.EquipmentDataSO.UpgradeAttackValue.ToString();
            criticalPercentText.text = equipmentInstance.EquipmentDataSO.UpgradeCriticalRate.ToString();
            criticalRatioText.text = equipmentInstance.EquipmentDataSO.UpgradeCriticalRatio.ToString();
            armorPenetrateText.text = equipmentInstance.EquipmentDataSO.UpgradeArmorPenetrate.ToString();
            infoText.text = "강화버튼을 누르면 강화를 진행합니다";
        }
        
        //업그레이드 버튼 비활성화 조건 체크
        long subMoney = CurrencyManager.Instance.gameMoney - cost;
        isEnoughMoney = subMoney >= 0;
        if(!isMaxLevel && isEnoughMoney)
        {
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeButton.interactable = false;
        }
    }

    public void ShowPanel(EquipmentInstance equipmentInfo, EquipmentInfoPanel infoPanel)
    {
        equipmentInstance = equipmentInfo;
        equipmentInfoPanel = infoPanel;
        equipmentImage.sprite = equipmentInstance.EquipmentDataSO.SpriteImage;

        UpdatePanel();
        this.gameObject.SetActive(true);
    }
}
