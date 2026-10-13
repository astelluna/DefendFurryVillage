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
    [SerializeField] List<TestEqipmentSerializeClass> testEqipmentSerializeClasseList = new List<TestEqipmentSerializeClass>(); //전체 아이템에 대한 정보를 직렬화해둔다.
    [SerializeField] Dictionary<string, EquipmentDataSO> equipmentItemDict = new(); //아이템별 어떤 SO를 쓰는지에 대한 정보

    //TODO. DB에서 가져와서 정보를 Set/Get 해주는거 필요
    //장비 갯수 관리 
    Dictionary<(string id, int level), int> equipmentCountDict = new Dictionary<(string id, int level), int>();
    //장비 특징 관리 
    Dictionary<(string id, int level), EquipmentInstance> equipmentInstanceDict = new Dictionary<(string id, int level), EquipmentInstance>();
    //캐릭터 관리
    Dictionary<string, CharacterEquipment> characterEquipmentDict = new Dictionary<string, CharacterEquipment>(); //캐릭터별 가지고있는 장비 정보

    //읽기 전용 딕셔너리
    public IReadOnlyDictionary<(string id, int level), int> EquipmentCountDict => equipmentCountDict;

    public int GetEquipmentCount(string id, int level)
    {
        return equipmentCountDict.GetValueOrDefault((id, level));
    }
    public void AddEquipmentCount(string id, int level, int amount)
    {
        equipmentCountDict[(id, level)] = GetEquipmentCount(id, level) + amount;
    }
    public bool GetEquipment(string id, int level)
    {
        if (GetEquipmentCount(id, level) <= 0) return false;
        equipmentCountDict[(id, level)]--;
        return true;
    }
    public CharacterEquipment GetCharacterEquipmentInfo(string characterId)
    {
        if(!characterEquipmentDict.TryGetValue(characterId, out CharacterEquipment characterEquipment))
        {
            characterEquipment = new CharacterEquipment();
            characterEquipmentDict[characterId] = characterEquipment;
        }
        return characterEquipment;
    }

    public EquipmentInstance GetEquipmentInstance(string id, int level)
    {
        if(!equipmentInstanceDict.TryGetValue((id,level),out EquipmentInstance equipmentInstance))
        {
            //새로 만들어준다.
            EquipmentDataSO equipmentData =  GetEquipmentData(id);
            if (equipmentData == null) { return null; }
            equipmentInstance = new EquipmentInstance(equipmentData, level);
            equipmentInstanceDict[(id, level)] = equipmentInstance;
        }
        return equipmentInstance;
    }


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
        test1.Add(0, 3); //0레벨 3개, 
        test1.Add(1, 4); //1레벨 4개,
        test1.Add(2, 5); //2레벨 5개,
        InitEquipmentHaveData("1000", test1); //아이템 1000이라는 id의 0,1,2레벨 템을 넣어줌
    }
    public EquipmentDataSO GetEquipmentData(string id)
    {
        if (!equipmentItemDict.ContainsKey(id)) return null;

        return equipmentItemDict[id];
    }
    public void InitEquipmentHaveData(string id, Dictionary<int,int> itemData)
    {
        //생성자로 알아서 EquipmentData가 정보 설정하도록 해줌
        foreach(var item in itemData)
        {
            int level = item.Key;
            int count = item.Value;
            AddEquipmentCount(id, level, count);

        }
    }
    public void AddEquipment(EquipmentInstance data, int amount)
    {
        //필요없을듯? AddEquipmentCount를 쓰면 될듯?;
        //장비 하나를 추가한다.
        string equipmentId = data.EquipmentDataSO.EquipmentId;
        int level = data.Level;

        AddEquipmentCount(equipmentId, level, amount);

    }

    public bool TryEquip(CharacterEquipment target, EquipmentInstance data)
    {
        //data에서 정보를 꺼내자
        string id = data.EquipmentDataSO.EquipmentId;
        int level = data.Level;
        int count = GetEquipmentCount(id, level);
        if (count <= 0) return false;
        var type = data.EquipmentDataSO.EquipmentType;
        var old = target.GetEquipment(type);
        //기존에 착용하던 장비를 빼서 인벤토리에 1개 채워준다.
        if (old != null) AddEquipmentCount(old.EquipmentDataSO.EquipmentId, old.Level, 1);
        //인벤토리에서 1개 빼서 장착해준다.
        if (GetEquipment(id,level))
        {
            target.SetEquipment(type, data);
            MyLog.Log("장착 성공");
            return true;
        }
        else
        {
            //장착 실패
            MyLog.Log("장착 실패");
            return false;
        }
    }

}
