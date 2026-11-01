using System;
using UnityEngine;

[Serializable]
public class ShopCashUIMaker
{
    //금액으로 구매하는 항목 전용 UI를 만들어주는 클래스
    [SerializeField] eProductType productType;
    [SerializeField] string productName;
    [SerializeField] Sprite productImage;
    [SerializeField] ShopItemUI shopItemUIPrefab;

    public eProductType ProductType => productType;

    public ShopItemUI MakeShopItemUI(Transform makePos)
    {
        ShopItemUI shopItemUIBuf = UnityEngine.Object.Instantiate(shopItemUIPrefab, makePos);
        ShopProduct shopProduct = new ShopProduct();
        Currency currency = new Currency();
        long price = GetPrice();
        currency.SetData(eCurrencyType.paidMoney, price);

        string id =  GetID();
        shopProduct.SetData(productType, id, productName, productImage, currency, 1);
        shopItemUIBuf.SetProductData(shopProduct);
        return shopItemUIBuf;
    }
    private long GetPrice()
    {
        switch(productType)
        {
            case eProductType.removeAds:
                return (long)IAPManager.Instance.RemoveAdsPrice;
            case eProductType.donate:
                return (long)IAPManager.Instance.DonatePrice;
        }
        return 0;
    }
    private string GetID()
    {
        switch(productType)
        {
            case eProductType.removeAds:
                return IAPManager.REMOVE_ADS_ID;
            case eProductType.donate:
                return IAPManager.DONATE_ID; ;
        }
        return "";
    }
}
