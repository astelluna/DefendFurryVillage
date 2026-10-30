using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>
{
    private const string REMOVE_ADS_ID = "remove_ads";
    private StoreController storeController;
    private Product removeAdsProduct;

    public bool IsAdRemoved { get; private set; }

    public event Action OnRemoveAdsProductReady;
    public event Action OnRemoveAdsPurchased; // 구매 완료 시 알림

    public string RemoveAdsPriceString => removeAdsProduct?.metadata.localizedPriceString;
    protected override void OnAwake()
    {
        InitializeIAP();
    }

    private async void InitializeIAP()
    {
        storeController = UnityIAPServices.StoreController();

        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnStoreDisconnected += OnStoreDisconnected;

        await storeController.Connect();

        var products = new List<ProductDefinition>
        {
            new ProductDefinition(REMOVE_ADS_ID, ProductType.NonConsumable)
        };

        storeController.FetchProducts(products);
    }

    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log("상품 불러오기 성공");
        removeAdsProduct = products.Find(p => p.definition.id == REMOVE_ADS_ID);
        OnRemoveAdsProductReady?.Invoke();
        storeController.FetchPurchases();
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsAdRemoved = false;
        foreach (var order in orders.ConfirmedOrders)
        {
            foreach (var item in order.CartOrdered.Items())
            {
                if (item.Product.definition.id == REMOVE_ADS_ID)
                {
                    IsAdRemoved = true;
                    Debug.Log("광고 제거 구매 상태 확인");
                    OnRemoveAdsPurchased?.Invoke(); // 기존 구매자도 알림 (패널 열 때 텍스트 갱신용)
                    return;
                }
            }
        }
        Debug.Log("광고 제거 미구매");
    }

    public void BuyRemoveAds()
    {
        if (storeController == null)
        {
            Debug.Log("IAP 초기화 중");
            return;
        }
        if (IsAdRemoved)
        {
            Debug.Log("이미 광고 제거를 구매했습니다.");
            return;
        }
        storeController.PurchaseProduct(REMOVE_ADS_ID);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            if (item.Product.definition.id == REMOVE_ADS_ID)
            {
                IsAdRemoved = true;
                Debug.Log("광고 제거 구매 완료");
                OnRemoveAdsPurchased?.Invoke(); // 구매 완료 알림
            }
        }
        storeController.ConfirmPurchase(order);
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.LogError($"상품 가져오기 실패 : {failure}");
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.LogError($"구매 내역 가져오기 실패 : {failure}");
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
    {
        Debug.LogError($"스토어 연결 실패 : {failure}");
    }


}