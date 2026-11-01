using System.Collections.Generic;
using UnityEngine;

public class ShopCashPanel : MonoBehaviour
{
    [SerializeField] ShopCashUIMaker[] productArray;
    List<ShopItemUI> shopItemList = new List<ShopItemUI>();

    [SerializeField] Transform makePosition; // UI들이 만들어지는 위치
    [SerializeField] ShopPurchaseConfirmPanel shopPurchaseConfirmPanel;
    [SerializeField] ShopBuyFailPanel shopBuyFailPanel;

    string curProductId = "";

    private void OnEnable()
    {
        IAPManager.Instance.OnRemoveAdsPurchased += PurchaseSuccess;
        IAPManager.Instance.OnDonatePurchased += PurchaseSuccess;
        IAPManager.Instance.OnPurchaseFailedEvent += PurchaseFail;
    }
    private void OnDisable()
    {
        Init();
        if (IAPManager.Instance == null) return;   // 종료 시 null 방지
        IAPManager.Instance.OnRemoveAdsPurchased -= PurchaseSuccess;
        IAPManager.Instance.OnDonatePurchased -= PurchaseSuccess;
        IAPManager.Instance.OnPurchaseFailedEvent -= PurchaseFail;
    }
    private void Init()
    {
        curProductId = "";
        foreach (var item in shopItemList)
        {
            if (item == null) continue;
            item.purchaseRequest -= ShowBuyProductInfo;
            Destroy(item.gameObject);
        }
        shopItemList.Clear();

    }
    private void UpdatePanel()
    {
        Init();
        //UI들 생성
        foreach(ShopCashUIMaker item in productArray)
        {
            if(item.ProductType == eProductType.removeAds)
            {
                if(IAPManager.Instance.IsAdRemoved)
                {
                    continue;
                }
            }
            ShopItemUI shopItem =  item.MakeShopItemUI(makePosition);
            shopItem.purchaseRequest += ShowBuyProductInfo;
            shopItemList.Add(shopItem);
        }
    }


    public void ShowPanel()
    {
        UpdatePanel();
        this.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }

    private void ShowBuyProductInfo(string productName, ShopProduct product)
    {
        curProductId = product.ProductId;
        string desc = $"{productName}을 {product.Price.Amount}로 구매하시겠습니까?";
        shopPurchaseConfirmPanel.ShowPanel(desc, BuyProcess);
    }
    private void BuyProcess()
    {
        if(curProductId == IAPManager.REMOVE_ADS_ID)
        {
            IAPManager.Instance.BuyRemoveAds();
        }
        else if (curProductId == IAPManager.DONATE_ID)
        {
            IAPManager.Instance.BuyDonate();
        }
    }
    private void PurchaseSuccess()
    {
        //끝나고나서 확인창 닫고, UI 갱신한번 해주자
        shopPurchaseConfirmPanel.ClosePanel();
        UpdatePanel();
    }
    private void PurchaseFail()
    {
        //끝나고나서 확인창 닫고, UI 갱신한번 해주자
        shopBuyFailPanel.ShowPanel();
        shopPurchaseConfirmPanel.ClosePanel();
        UpdatePanel();
    }

}
