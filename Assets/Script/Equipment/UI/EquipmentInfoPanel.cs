using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] Button mountButton;
    [SerializeField] Button closeButton;

    [SerializeField] Button upgradeButton; //장비 강화 버튼
    [SerializeField] EquipmentUpgradePanel upgradePanel;

    public event Action<EquipmentInstance> MountEquipmentRequest;
    public event Action UpdatePanelRequest;
    EquipmentInstance equipmentInstance;
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        mountButton.onClick.AddListener(OnMountButtonClick);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        mountButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();
    }
    private void OnUpgradeButtonClick()
    {
        upgradePanel.ShowPanel(equipmentInstance, this);
    }

    public void ClosePanel()
    {
        OnCloseButtonClick();
    }
    public void OnRequestUpdate()
    {
        UpdatePanelRequest.Invoke();
        ClosePanel();
    }

    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    private void OnMountButtonClick()
    {
        int count = EquipmentManager.Instance.GetEquipmentCount(equipmentInstance.EquipmentDataSO.Id, equipmentInstance.Level);

        if(count <= 0)
        {
            MyLog.LogError("갯수가 0인데 장착하려고 합니다. 에러");
            return;
        }

        MountEquipmentRequest?.Invoke(equipmentInstance);
        this.gameObject.SetActive(false);
    }

    //public void SetAndShowPanel(string name, string desc, string level)
    public void SetAndShowPanel(EquipmentInstance data)
    {
        int count = EquipmentManager.Instance.GetEquipmentCount(data.EquipmentDataSO.Id, data.Level);

        equipmentInstance = data;
        itemNameText.text = data.EquipmentDataSO.DisplayName;
        itemDescText.text = data.GetEquipmentDescription();
        countText.text = count.ToString();
        levelText.text = $"{data.Level.ToString()}";
        this.gameObject.SetActive(true);
    }

}
