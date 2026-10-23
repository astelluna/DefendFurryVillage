using System.Collections.Generic;
using UnityEngine;

public class ShopCharacterPanel : MonoBehaviour
{
    //구매가능한 캐릭터 목록을 띄우는 패널
    [SerializeField] ShopItemUI shopItemUIPrefab;
    [SerializeField] Transform makePosition;

    [SerializeField] ShopPanel shopPanel;

    List<ShopItemUI> makedCharacterItemList = new List<ShopItemUI>();

    private void OnDisable()
    {
        //패널이 서로 교체될때 비활성화 됨, 그떄마다 초기화를 계속 해줘야함
        Init();
    }

    private void Init()
    {
        //아이템 리스트 초기화
        foreach (ShopItemUI shopItem in makedCharacterItemList)
        {
            shopItem.purchaseRequest -= shopPanel.PurchaseProcess;
            Destroy(shopItem.gameObject);
        }
        makedCharacterItemList.Clear();
    }
    public void UpdatePanel()
    {
        Init();
        MakeUI();
    }
    public void ShowPanel()
    {
        if (this.gameObject.activeInHierarchy)  { return; } //이미 열려있다면 안해줌

        MakeUI();
        this.gameObject.SetActive(true);
    }
    private void MakeUI()
    {
        //캐릭터 구매 패널 클릭 -> 구매할수 있는 캐릭터 목록이 나온다
        IReadOnlyDictionary<string, CharacterDataSO> unHaveCharacterDict = CharacterManager.Instance.UnHaveCataracterDict;
        foreach (KeyValuePair<string, CharacterDataSO> pair in unHaveCharacterDict)
        {
            //TODO. 추후 오브젝트풀로 할거임
            ShopItemUI shopItem = Instantiate(shopItemUIPrefab, makePosition);
            shopItem.purchaseRequest += shopPanel.PurchaseProcess;
            //상점 품목 데이터를 만든다
            ShopProduct shopProduct = new ShopProduct();
            shopProduct.SetData(eProductType.character, pair.Value.CharacterId, pair.Value.CharacterName, pair.Value.PurchasePrice, 1);
            shopItem.SetProductData(shopProduct);
            makedCharacterItemList.Add(shopItem);
        }
    }

    public void ClosePanel()
    {
        Init();
        this.gameObject.SetActive(false);
    }
}
