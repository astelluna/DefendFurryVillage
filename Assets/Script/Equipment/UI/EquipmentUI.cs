using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    //인벤토리에 표시되는 장비
    //[SerializeField] 
    [SerializeField] Button button; //자기자신
    [SerializeField] TextMeshProUGUI equipmentCountText;

    EquipmentInstance levelData;

    public event Action<EquipmentInstance> OnClickUI;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetData(EquipmentInstance data)
    {
        int count = ItemManager.Instance.GetEquipmentCount(data.EquipmentDataSO.EquipmentId, data.Level);
        levelData = data;
        equipmentCountText.text = count.ToString();
        button.image.sprite = data.EquipmentDataSO.EquipmentImage;
    }

    private void OnButtonClicked()
    {
        OnClickUI?.Invoke(levelData);
    }
}
