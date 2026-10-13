using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    //캐릭터마다 장착중인 이이템을 저장하는 클래스]
    Dictionary<eEquipmentType, EquipmentInstance> characterEquipmentDict = new Dictionary<eEquipmentType, EquipmentInstance>();
    public EquipmentInstance GetEquipment(eEquipmentType type)
    {
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
