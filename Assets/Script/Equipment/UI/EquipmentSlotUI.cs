using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [field: SerializeField] public eEquipmentType EquipmentType { get; private set; } //이 슬롯은 어떤 장비 타입이 들어가는지 체크
    [SerializeField] Button slotButton; //자기자신 버튼
    [SerializeField] TextMeshProUGUI equipmentLevelText;
    [SerializeField] Sprite baseSprite;
    public EquipmentInstance equipmentInstanceData { get; private set; }

    //public event Action<EquipmentLevelData> EquipmentLevelDataRequest;
    public event Action<EquipmentSlotUI> EquipmentSlotUIRequest;

    private void Awake()
    {
        slotButton.onClick.AddListener(OnSlotButtonClick);
        ClearData();
    }
    private void OnDestroy()
    {
        slotButton.onClick.RemoveAllListeners();
    }
    private void OnSlotButtonClick()
    {
        //해제할건지 물어보는 패널이 아래에 뜰거임.
        //equipmentUnEquipPanel.ShowPanel();
        EquipmentSlotUIRequest?.Invoke(this);
    }

    public void SetData(EquipmentInstance data)
    {
        equipmentInstanceData = data;
        slotButton.image.sprite = equipmentInstanceData.EquipmentDataSO.EquipmentImage;
        equipmentLevelText.text = "+" + equipmentInstanceData.Level;

    }
    public void ClearData()
    {
        equipmentInstanceData = null;
        slotButton.image.sprite = baseSprite;
        equipmentLevelText.text = "";
    }


}