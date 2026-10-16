using System.Collections.Generic;
using UnityEngine;

public class ItemInstance
{
    public ItemDataSO ItemSO { get; private set; }
    public int Count { get; private set; }

    public ItemInstance(ItemDataSO so, int count)
    {
        ItemSO = so;
        Count = count;
    }

    public void AddCount(int amount)
    {
        int result = Count + amount;
        //Count += amount;
        if(result < 0)
        {
            MyLog.LogError("갯수가 0을 넘어서 들어왔습니다. 에러");
            result = 0;
        }
        Count = result;
    }
}

public class ItemManager : Singleton<ItemManager>
{
    //유저가 가지고 있는 아이템중 장비를 제외한 (소모품, 기타) 아이템들이 여기에 저장될거임.
    //TODO. 나중에 데이터베이스 SO 및 에디터 스크립트로 존재하는 모든 ItemDataSO 가져오게 하자
    [SerializeField] List<ItemDataSO> allItemDataList = new List<ItemDataSO>();
    Dictionary<string, ItemDataSO> allItemDataDict;

    //아이템 인스턴스를 만들어서 현재 몇개를 가지고있는지 갯수를 저장
    private List<ItemInstance> itemInstanceList = new List<ItemInstance>();

    protected override void OnAwake()
    {
    }
    public void InitManager(List<ItemDbInfo> itemDbInfoList)
    {
        InitData();

        itemInstanceList.Clear();
        HashSet<string> itemIdHash = new HashSet<string>();
        foreach (var item in itemDbInfoList)
        {
            //2.So매니저? 거기에서 검색해서 있으면 그 타입이다 라는걸 알수있다. 그 아이템으로 정보를 변환한다, 대신 전체아이템SO에 대해서 직렬화 해둬야함.

            if(itemIdHash.Contains(item.ItemId))
            {
                MyLog.LogError("DB에 아이템이 중복으로 있습니다. 뭔가 에러");
                continue;
            }
            if(!allItemDataDict.TryGetValue(item.ItemId, out ItemDataSO itemDataSO))
            {
                MyLog.LogError($"{item.ItemId}가 가지고 있는 아이템 목록에 없습니다");
                continue;
            }
            itemIdHash.Add(item.ItemId);
            ItemInstance itemInstance = new ItemInstance(itemDataSO, item.Count);
            itemInstanceList.Add(itemInstance);
        }
    }
    private void InitData()
    {
        if (allItemDataDict == null)
        {
            //초기 1회 사용중이라면 초기화
            foreach(var item in allItemDataList)
            {
                if (allItemDataDict.ContainsKey(item.ItemId))
                {
                    MyLog.LogError($"{item.name}항목의 {item.ItemId}의 중복된 아이템키의 아이템이 있습니다");
                    continue;
                }

                allItemDataDict.Add(item.ItemId, item);
            }
        }
    }
}
