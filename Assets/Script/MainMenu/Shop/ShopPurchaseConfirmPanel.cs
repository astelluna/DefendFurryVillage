using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPurchaseConfirmPanel : MonoBehaviour
{
    //상점에서 구매버튼 누르면 정말 구매할건지 확인해주는 패널
    [SerializeField] Button confirmButton;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Button closeButton;

    Action purchaseRequest;
    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    private void OnDestroy()
    {
        confirmButton.onClick.RemoveAllListeners();
    }
    private void OnConfirmButtonClicked()
    {
        purchaseRequest?.Invoke();
    }
    private void OnCloseButtonClicked()
    {
        purchaseRequest = null;
        this.gameObject.SetActive(false);
    }
    public void ShowPanel(string desc ,Action action)
    {
        descText.text = desc;
        purchaseRequest = action;
        this.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        OnCloseButtonClicked();
    }

}
