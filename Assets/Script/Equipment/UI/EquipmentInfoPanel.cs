using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnClose);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    private void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    public void SetAndShowPanel(string name, string desc, string level)
    {
        itemNameText.text = name;
        itemDescText.text = desc;
        levelText.text = $"업그레이드 단계: {level}";
        this.gameObject.SetActive(true);
    }

}
