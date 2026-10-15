using Firebase.Firestore;
using UnityEngine;
[FirestoreData]
public class CharacterLevelDataDB
{
    //DB에서 불러온 캐릭터 레벨 데이터
    [FirestoreProperty] public string id { get; set; }
    [FirestoreProperty]public int level { get; set; }
    [FirestoreProperty]public int exp { get; set; }
    [FirestoreProperty]public CharacterEquipmentDataDB WeaponEquipment { get; set; }
    [FirestoreProperty]public CharacterEquipmentDataDB ArmorEquipment { get; set; }
    [FirestoreProperty] public CharacterEquipmentDataDB AccessoriesEquipment { get; set; }
    public CharacterLevelDataDB(string characterId, int characterLevel, int characterExp)
    {
        id = characterId;
        level = characterLevel;
        exp = characterExp;

        WeaponEquipment = new CharacterEquipmentDataDB();
        ArmorEquipment = new CharacterEquipmentDataDB();
        AccessoriesEquipment = new CharacterEquipmentDataDB();
    }
}
[FirestoreData]
public class CharacterEquipmentDataDB
{
    [FirestoreProperty] public string equipmentId { get; set; }
    [FirestoreProperty] public int equipmentLevel { get; set; }
}