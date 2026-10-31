using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    //상점에 표시되는 아이템 항목을 관리
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] Button itemPurchaseButton;
    [SerializeField] TextMeshProUGUI itemPurchaseButtonText; //버튼의 텍스트

    //Currency currency; //이벤트로 이걸 보내줄거임. 버튼 누르면 얼마인지.
    ShopProduct shopProduct;
    public event Action<string, ShopProduct> purchaseRequest;

    private void Awake()
    {
        itemPurchaseButton.onClick.AddListener(OnItemPurchaseButtonClick);
    }
    private void OnDestroy()
    {
        itemPurchaseButton.onClick.RemoveAllListeners();
    }
    private void OnItemPurchaseButtonClick()
    {
        purchaseRequest?.Invoke(itemNameText.text, shopProduct);
    }
    public void SetProductData(ShopProduct product)
    {
        shopProduct = product;
        string currencyString = "";
        if(shopProduct.Price.CurrencyType == eCurrencyType.gameMoney)
        {
            currencyString = "G";
        }
        else
        {
            currencyString = "₩";
        }
        
        
        itemNameText.text = shopProduct.DisplayName;
        itemDescText.text = shopProduct.DisplayName + "를 구매합니다";
        itemImage.sprite = shopProduct.ProductImage;
        itemPurchaseButtonText.text = currencyString + " " + shopProduct.Price.Amount.ToString();

    }
}
