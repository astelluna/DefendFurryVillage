using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum eProductType
{
    character,
    removeAds,
    donate,
    consumable
}
public struct ShopProduct
{
    //상점에서 구매하는 물품에 대한 정보
    public eProductType ProductType { get; private set; }
    public string ProductId { get; private set; }
    public string DisplayName { get; private set; }
    public Currency Price { get; private set; } //가격
    public int Count { get; private set; } //지급 갯수
    public void SetData(eProductType type, string id, string name, Currency currency, int cnt)
    {
        ProductType = type;
        ProductId = id;
        DisplayName = name;
        Price = currency;
        Count = cnt;
    }
}

public class ShopPanel : MonoBehaviour, IMainMenuPanel
{
    //상점 구조를 만들어야한다. 
    //필요한것 : 캐릭터 구매. 일단 이걸로 끝

    //미보유 캐릭터를 전부 불러온다; 그리고 표시한다; 근데 가격은?;

    [SerializeField] Button showCharacterPanelButton;
    [SerializeField] Button showCashPanelButton;

    [Header("후원 상점 목록 - 고정")]
    //TODO. 추후 만들거(캐릭터와는 다르게 고정으로 UI를 생성해야한다.)
    [SerializeField] ShopItemUI removeAddSlot;
    [SerializeField] ShopItemUI donationSlot;
    [Header("패널들")]
    [SerializeField] ShopPurchaseConfirmPanel shopPurchaseConfirmPanel; //구매 확인 패널
    [SerializeField] ShopCharacterPanel shopPurchasableCharacterPanel; //구매가능한 캐릭터를 띄우는 패널
    private void Awake()
    {
        //TODO. 패널이 열릴떄, 후원과, 광고제거 패키지는 구매를 했는지 초기 1회 체크가 필요함.
        showCharacterPanelButton.onClick.AddListener(ShowCharacterPanelButtonClicked);
    }
    private void OnDestroy()
    {
        showCharacterPanelButton.onClick.RemoveAllListeners();
    }
    public void PurchaseProcess(string itemName, ShopProduct product)
    {
        // 1. 구매가 가능한지 체크가 필요.
        if(product.Price.CurrencyType == eCurrencyType.gameMoney)
        {
            if(!CurrencyManager.Instance.CanBuyProduct(product.Price.Amount))
            {
                MyLog.LogError("결제 금액 부족으로 구매불가");
                return;
            }
        }
        else if(product.Price.CurrencyType == eCurrencyType.paidMoney)
        {
            //TODO.실제 돈이라면? 결제창을 통해 결제를 진행하고. 다른 루틴을 타야할듯?
            MyLog.LogError("아직 구현 안함");
            return;
        }

        //2. 구매 가능하므로 구매 진행
        string itemTypeString = "";
        switch (product.Price.CurrencyType)
        {
            case eCurrencyType.gameMoney:
                itemTypeString = "골드";
                break;
            case eCurrencyType.paidMoney:
                itemTypeString = "원";
                break;
        }
        

        string msg = $"{itemName}을 {product.Price.Amount} {itemTypeString}으로 구매하시겠습니까?";
        ShopProduct productBuf = product;
        shopPurchaseConfirmPanel.ShowPanel(msg, () => PurchaseItem(productBuf));
    }
    private async void PurchaseItem(ShopProduct shopProduct)
    {
        //돈 차감되고, 아이템을 획득하도록 진행

        bool purchaseSuccess = false;
        //구매 진행.
        MyLog.Log($"{shopProduct.Price.CurrencyType} {shopProduct.Price.Amount} 구매");

        switch(shopProduct.ProductType)
        {
            case eProductType.character:
                purchaseSuccess = await PurcharseCharacter(shopProduct);
                break;
            case eProductType.donate:
                //TODO. 후원
                break;
            case eProductType.removeAds:
                //TODO. 광고제거
                break;
        }
        if (!purchaseSuccess)
        {
            MyLog.LogError($"{shopProduct.ProductType}항목 구매 실패");
            return;
        }
        else
        {
            MyLog.Log($"{shopProduct.ProductType}항목 구매 완료");
        }
        //끝나고나서 확인창 닫고, UI 갱신한번 해주자
        shopPurchaseConfirmPanel.ClosePanel();
        shopPurchasableCharacterPanel.UpdatePanel();
    }
    private async Task<bool> PurcharseCharacter(ShopProduct product)
    {
        //캐릭터를 구매하면 어떻게? 
        //DB에 캐릭터 하나 추가. 런타임에도 캐릭터 하나 추가할거임. 
        bool isSusscess = await FirestoreManager.Instance.PurchaseProduct_Character(product);
        return isSusscess;
    }



    
    private void ShowCharacterPanelButtonClicked()
    {
        shopPurchasableCharacterPanel.ShowPanel();
    }

    public void ShowPanel(bool isShow)
    {
        if(!isShow)
        {
            shopPurchasableCharacterPanel.ClosePanel();
            shopPurchaseConfirmPanel.ClosePanel();
        }
        else
        {
            //기본 화면은 캐릭터로 하자
            ShowCharacterPanelButtonClicked();
        }
        this.gameObject.SetActive(isShow);
    }
}
