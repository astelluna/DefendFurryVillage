using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Button mountButton;
    [SerializeField] Button closeButton;

    public event Action<EquipmentInstance> MountEquipmentRequest;
    EquipmentInstance equipmentLevelData;
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        mountButton.onClick.AddListener(OnMountButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        mountButton.onClick.RemoveAllListeners();
    }

    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    private void OnMountButtonClick()
    {
        int count = ItemManager.Instance.GetEquipmentCount(equipmentLevelData.EquipmentDataSO.EquipmentId, equipmentLevelData.Level);

        if(count <= 0)
        {
            MyLog.LogError("갯수가 0인데 장착하려고 합니다. 에러");
            return;
        }

        MountEquipmentRequest?.Invoke(equipmentLevelData);
        this.gameObject.SetActive(false);
    }

    //public void SetAndShowPanel(string name, string desc, string level)
    public void SetAndShowPanel(EquipmentInstance data)
    {
        equipmentLevelData = data;
        itemNameText.text = data.EquipmentDataSO.EquipmentName;
        itemDescText.text = data.GetEquipmentDescription();
        levelText.text = $"업그레이드 단계: {data.Level.ToString()}";
        this.gameObject.SetActive(true);
    }

}
