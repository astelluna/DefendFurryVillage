using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EquipmentLevelData
{
    //현재 장비 데이터
    public EquipmentDataSO EquipmentDataSO;
    public int Level { get; private set; }
    public int Count { get; private set; }
    RuntimeEquipmentData runtimeEqipmentData = new RuntimeEquipmentData();

    public EquipmentLevelData(EquipmentDataSO data, int lvl, int cnt)
    {
        EquipmentDataSO = data;
        Level = lvl;
        Count = cnt;
        runtimeEqipmentData.UpdateData(EquipmentDataSO, Level);
    }

    public string GetEquipmentDescription()
    {
        string msg = "";

        if (runtimeEqipmentData.attack > 0)
        {
            msg += $" 공격력을 {runtimeEqipmentData.attack},";
        }
        if (runtimeEqipmentData.criticalRate > 0)
        {
            msg += $" 크리티컬 확률을 {runtimeEqipmentData.criticalRate},";
        }
        if (runtimeEqipmentData.criticalRatio > 0)
        {
            msg += $" 크리티컬 데미지 배율을 {runtimeEqipmentData.criticalRatio},";
        }
        if (runtimeEqipmentData.armorPenetrate > 0)
        {
            msg += $" 방어구 관통을 {runtimeEqipmentData.armorPenetrate},";
        }
        msg = msg.TrimEnd(',');
        msg += " 만큼 증가시킵니다";
        return msg;
    }
}
public class EquipmentData
{
    public string eqipmentId;
    Dictionary<int, EquipmentLevelData> equipmentLevelDataDict = new();
    public IReadOnlyDictionary<int, EquipmentLevelData> EquipmentLevelDataDict => equipmentLevelDataDict;

    public EquipmentData(string id, Dictionary<int, int> dict)
    {
        //어떤 정보를 가져와야 가능할까? -> DB에서 string id값 하나랑, Dictionary<int,int> 값으로 업그레이드번호, 갯수를 만들거임
        equipmentLevelDataDict.Clear();
        eqipmentId = id;
        foreach (var item in dict)
        {
            int level = item.Key;
            int count = item.Value;
            MakeData(id, level, count);
        }
    }

    public void MakeData(string id,int level, int count)
    {
        //TODO. 추후 UI에서 어떤 항목 클릭했을때 해당 데이터의 EquipmentDataClass를 가져온다음에. 그거로 처리.
        eqipmentId = id;
        EquipmentDataSO data = ItemManager.Instance.GetEquipmentData(id);
        EquipmentLevelData dataClass = new EquipmentLevelData(data, level,count);
        if (!equipmentLevelDataDict.ContainsKey(level))
        {
            equipmentLevelDataDict.Add(level, dataClass);
        }
        else
        {
            equipmentLevelDataDict[level] = dataClass;
        }

    }

}
