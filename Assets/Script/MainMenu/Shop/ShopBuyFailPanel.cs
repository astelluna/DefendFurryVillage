using UnityEngine;
using UnityEngine.UI;

public class ShopBuyFailPanel : MonoBehaviour
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
        this.gameObject.SetActive(false);
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
}
