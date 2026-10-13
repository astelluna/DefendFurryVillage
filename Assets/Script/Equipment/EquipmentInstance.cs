using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EquipmentInstance
{
    //기본규칙: 런타임에서 바뀌는 데이터는 넣지말자. Count 같은거. 레벨별 데이터를 쉽게 계산할수있게 하는 인스턴스.
    //현재 장비 데이터
    public EquipmentDataSO EquipmentDataSO;
    public int Level { get; private set; }
    //public int Count { get; private set; }
    RuntimeEquipmentData runtimeEqipmentData = new RuntimeEquipmentData();

    public EquipmentInstance(EquipmentDataSO data, int lvl)
    {
        EquipmentDataSO = data;
        Level = lvl;
        runtimeEqipmentData.UpdateData(EquipmentDataSO, Level);
    }

    public string GetEquipmentDescription()
    {
        string msg = "";

        if (runtimeEqipmentData.attack > 0)
        {
            msg += $" 공격력을 {runtimeEqipmentData.attack},";
        }
        if (runtimeEqipmentData.criticalpercent > 0)
        {
            msg += $" 크리티컬 확률을 {runtimeEqipmentData.criticalpercent},";
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
