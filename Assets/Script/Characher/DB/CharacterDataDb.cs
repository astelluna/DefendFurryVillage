using Firebase.Firestore;
using System.Collections.Generic;
[FirestoreData]
public class CharacterDataDb
{
    //DB에서 불러온 캐릭터 레벨 데이터
    [FirestoreProperty] public string id { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int exp { get; set; }


    //아래 항목 3개를 딕셔너리로 바꿀거임. 그래서 특정 아이템이 있는지 확인할수있게 ;
    //[FirestoreProperty] public CharacterEquipmentDataDB WeaponEquipment { get; set; }
    //[FirestoreProperty] public CharacterEquipmentDataDB ArmorEquipment { get; set; }
    //[FirestoreProperty] public CharacterEquipmentDataDB AccessoriesEquipment { get; set; }

    //아이템이 존재하지않으면 애초에 DB에 저장이 안됨.
    [FirestoreProperty] public Dictionary<string, CharacterEquipmentDataDB> EquipmentDict { get; set; }



    public CharacterDataDb()
    {
        //기본 생성자가 Load 자동변환을 위해 반드시 필요함
        EquipmentDict = new Dictionary<string, CharacterEquipmentDataDB>();
    }

    public CharacterDataDb(string characterId, int characterLevel, int characterExp)
    {
        id = characterId;
        level = characterLevel;
        exp = characterExp;
        EquipmentDict = new Dictionary<string, CharacterEquipmentDataDB>();
    }
    private string ParseEquipmentEnum(eEquipmentType type)
    {
        return type.ToString();
    }

    public CharacterEquipmentDataDB GetCharacterEquipmentDataDB(eEquipmentType type)
    {
        string equipmentKey = ParseEquipmentEnum(type);
        if (!EquipmentDict.ContainsKey(equipmentKey))
        {
            return null;
        }
        return EquipmentDict[equipmentKey];

    }

    public bool SetCharacterEquipmentDB(eEquipmentType type, EquipmentInstance equipmentInstance)
    {
        CharacterEquipmentDataDB dataDB = null;
        string equipmentKey = ParseEquipmentEnum(type);
        if (EquipmentDict.ContainsKey(equipmentKey))
        {
            dataDB = EquipmentDict[equipmentKey];
        }
        else
        {
            dataDB = new CharacterEquipmentDataDB();
            EquipmentDict.Add(equipmentKey, dataDB);
        }
        dataDB.setData(equipmentInstance.EquipmentDataSO.EquipmentId, equipmentInstance.Level);
        MyLog.Log("DB에 올리기전 - 설정 완료");
        return true;
    }
    public CharacterEquipmentDataDB UnEquipmentCharacterDB(eEquipmentType type)
    {
        //특정 타입의 장비를 해제시킨다.
        string equipmentKey = ParseEquipmentEnum(type);
        if (!EquipmentDict.ContainsKey(equipmentKey)) { return null; }

        CharacterEquipmentDataDB characterEquipmentDataDB = EquipmentDict[equipmentKey];
        EquipmentDict.Remove(equipmentKey);
        return characterEquipmentDataDB;
    }
}
[FirestoreData]
public class CharacterEquipmentDataDB
{
    [FirestoreProperty] public string equipmentId { get; set; }
    [FirestoreProperty] public int equipmentLevel { get; set; }
    public void setData(string id, int level)
    {
        equipmentId = id;
        equipmentLevel = level;
    }
}