using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    //인벤토리에 표시되는 장비
    //[SerializeField] 
    [SerializeField] Button button; //자기자신
    //[SerializeField] TextMeshProUGUI equipmentCountText;
    [SerializeField] TextMeshProUGUI equipmentLevelText;

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
        int count = EquipmentManager.Instance.GetEquipmentCount(data.EquipmentDataSO.Id, data.Level);
        levelData = data;
        //equipmentCountText.text = count.ToString();
        equipmentLevelText.text = "+" + levelData.Level.ToString();
        button.image.sprite = data.EquipmentDataSO.SpriteImage;
    }

    private void OnButtonClicked()
    {
        OnClickUI?.Invoke(levelData);
    }
}
