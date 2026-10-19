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
    public CharacterStat GetEquipmentStat()
    {
        //현재 장착중인 모든 장비의 스텟을 더해서 클래스 형태로 반환해준다.
        CharacterStat characterStat = new CharacterStat();
        if (characterEquipmentDict.Count == 0) 
        {
            characterStat.SetZero();
            return characterStat; 
        }
        int atk = 0;
        int critPercent = 0;
        float critRate = 0f;
        float armorpenet = 0f;

        foreach(var pair in characterEquipmentDict)
        {
            RuntimeEquipmentData runtimeEquipmentData =  pair.Value.GetRuntimeEquipmentData();
            atk += runtimeEquipmentData.attack;
            critPercent += runtimeEquipmentData.criticalpercent;
            critRate += runtimeEquipmentData.criticalRatio;
            armorpenet += runtimeEquipmentData.armorPenetrate;

        }
        //최종 스텟으로 스텟을 만들어서 반환해준다.
        characterStat.SetData(atk, critPercent, critRate, armorpenet);
        return characterStat;
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
