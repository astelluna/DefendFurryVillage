using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>
{
    public const string REMOVE_ADS_ID = "remove_ads";
    public const string DONATE_ID = "donate";
    private StoreController storeController;
    private Product removeAdsProduct;
    private Product donateProduct;

    public bool IsAdRemoved { get; private set; }
    private bool IsRemoveAdsReady => removeAdsProduct != null;
    private bool IsDonateReady => donateProduct != null;

    public event Action OnProductReady;
    public event Action OnRemoveAdsPurchased; // 구매 완료 시 알림 - 광고제거
    public event Action OnDonatePurchased; // 구매 완료 시 알림 - 후원
    public event Action OnPurchaseFailedEvent; //구매 실패시 알림

    public decimal RemoveAdsPrice => removeAdsProduct?.metadata.localizedPrice ?? 0m;
    public decimal DonatePrice => donateProduct?.metadata.localizedPrice ?? 0m;
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
        storeController.OnPurchaseFailed += OnPurchaseFailed;

        await storeController.Connect();

        var products = new List<ProductDefinition>
        {
            new ProductDefinition(REMOVE_ADS_ID, ProductType.NonConsumable),
            new ProductDefinition(DONATE_ID, ProductType.Consumable)

        };

        storeController.FetchProducts(products);
    }

    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log("상품 불러오기 성공");
        removeAdsProduct = products.Find(p => p.definition.id == REMOVE_ADS_ID);
        donateProduct = products.Find(p => p.definition.id == DONATE_ID);

        OnProductReady?.Invoke();
        storeController.FetchPurchases();
    }
    private void OnPurchaseFailed(FailedOrder order)
    {
        Debug.LogError($"구매 실패/취소 : {order}");
        OnPurchaseFailedEvent?.Invoke();
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
                }
                //후원은 여러번 할수있게 하자
            }
        }
        if (IsAdRemoved)
        {
            Debug.Log("광고 제거 구매 상태 확인");
            OnRemoveAdsPurchased?.Invoke(); // 기존 구매자도 알림 (패널 열 때 텍스트 갱신용)
        }
        else
        {
            Debug.Log("광고 제거 미구매");
        }
    }

    public void BuyRemoveAds()
    {
        if (!IsRemoveAdsReady)
        {
            Debug.Log("광고 제거 상품 준비 안 됨");
            return;
        }
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
    public void BuyDonate()
    {
        if (!IsDonateReady)
        {
            Debug.Log("후원 상품 준비 안 됨");
            return;
        }
        if (storeController == null)
        {
            Debug.Log("IAP 초기화 중");
            return;
        }
        storeController.PurchaseProduct(DONATE_ID);
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
            else if (item.Product.definition.id == DONATE_ID)
            {
                Debug.Log("후원 구매 완료");
                OnDonatePurchased?.Invoke(); // 구매 완료 알림
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