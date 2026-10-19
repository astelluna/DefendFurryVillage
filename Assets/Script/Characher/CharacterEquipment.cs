using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment
{
    //캐릭터마다 장착중인 이이템을 저장하는 클래스]
    Dictionary<eEquipmentType, EquipmentInstance> characterEquipmentDict = new Dictionary<eEquipmentType, EquipmentInstance>();

    public void UpdateCharacterEquipment_DB(CharacterDataDb characterDataDb)
    {
        characterEquipmentDict.Clear();
        //DB정보를 통해 캐릭터 장착 정보를 모두 갱신
        foreach (var item in characterDataDb.EquipmentDict)
        {
            string typeString = item.Key;

            if (!Enum.TryParse<eEquipmentType>(typeString, out eEquipmentType type))
            {
                MyLog.LogError($"{typeString}의 string값을 enum으로 변환실패. ");
                continue;
            }
            EquipmentInstance equipmentInstance = EquipmentManager.Instance.GetEquipmentInstance(item.Value.equipmentId, item.Value.equipmentLevel);
            characterEquipmentDict.Add(type, equipmentInstance);
        }

    }


    public EquipmentInstance GetEquipment(eEquipmentType type)
    {
        //장비가 있을경우에는 반환, 없으면 null 자체를 반환
        if (characterEquipmentDict.TryGetValue(type, out EquipmentInstance equipment)) 
        {
            return equipment;
        }
        return null;
    }
    public void SetEquipment(eEquipmentType type, EquipmentInstance equipmentData)
    {
        characterEquipmentDict[type] = equipmentData;
    }
    public void RemoveEquipment(eEquipmentType type)
    {
        characterEquipmentDict.Remove(type);
    }

}
