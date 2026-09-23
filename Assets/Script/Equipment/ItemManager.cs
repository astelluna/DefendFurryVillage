using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestEqipmentSerializeClass
{
    public string id;
    public EquipmentDataSO dataSO;
}

public class ItemManager : Singleton<ItemManager>
{
    //TODO. 일단 직렬화. 나중에는? 나중에도 직렬화?
    [SerializeField] List<TestEqipmentSerializeClass> testEqipmentSerializeClasseList = new List<TestEqipmentSerializeClass>();
    [SerializeField] Dictionary<string, EquipmentDataSO> equipmentItemDict = new(); //아이템별 어떤 SO를 쓰는지에 대한 정보

    //TODO. DB에서 가져와서 정보를 Set/Get 해주는거 필요
    Dictionary<string, EquipmentData> equipmentHaveDataDict =new Dictionary<string, EquipmentData>(); //현재 아이템별 가지고있는 정보. 몇개를 가지고있는지
    public IReadOnlyDictionary<string, EquipmentData> EquipmentHaveDataDict => equipmentHaveDataDict;

    protected override void OnAwake()
    {
        
    }
    private void Start()
    {
        //TODO. 추후에는 DB에서 받아서 만들어야함
        //테스트용 데이터 넣기 
        foreach(var item in testEqipmentSerializeClasseList)
        {
            equipmentItemDict.Add(item.id, item.dataSO);
        }

        //TODO. 일단 수동으로 호출해보자. 
        Dictionary<int, int> test1 = new Dictionary<int, int>();
        test1.Add(0, 3);
        test1.Add(1, 4);
        test1.Add(2, 5);
        InitEquipmentHaveData("1000", test1);
    }
    public EquipmentDataSO GetEquipmentData(string id)
    {
        if (!equipmentItemDict.ContainsKey(id)) return null;

        return equipmentItemDict[id];
    }
    public void InitEquipmentHaveData(string id, Dictionary<int,int> itemData)
    {
        //생성자로 알아서 EquipmentData가 정보 설정하도록 해줌
        EquipmentData equipmentData = new EquipmentData(id, itemData);
        if(equipmentHaveDataDict.ContainsKey(id))
        {
            MyLog.LogError("아이디값 중복. 확인해주세요");
            return;
        }
        equipmentHaveDataDict.Add(id, equipmentData);
    }


}
