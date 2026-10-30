using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnCloseButtonClicked()
    {
        ClosePanel();
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }
}
