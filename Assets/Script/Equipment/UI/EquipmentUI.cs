using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    //인벤토리에 표시되는 장비
    //[SerializeField] 
    Button button; //자기자신
    [SerializeField] TextMeshProUGUI equipmentCountText;

    EquipmentLevelData levelData;

    public event Action<EquipmentLevelData> OnClickUI;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetData(EquipmentLevelData data)
    {
        levelData = data;
        equipmentCountText.text = levelData.Count.ToString();
        button.image.sprite = data.EquipmentDataSO.EquipmentImage;
    }

    private void OnButtonClicked()
    {
        OnClickUI?.Invoke(levelData);
    }
}
