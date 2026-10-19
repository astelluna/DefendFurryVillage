using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EquipmentInstance
{
    //캐릭터가 어떤 장비를 장착했는지 정보를 표시하기위한 클래스, 생성이나 가져올때 EqipmentManager에서 가져오면 됨
    //기본규칙: 런타임에서 바뀌는 데이터는 넣지말자. Count 같은거. 레벨별 데이터를 쉽게 계산할수있게 하는 인스턴스.
    public EquipmentDataSO EquipmentDataSO; //현재 장비 데이터
    public int Level { get; private set; }
    //public int Count { get; private set; }
    RuntimeEquipmentData runtimeEqipmentData = new RuntimeEquipmentData();


    public void SetData(CharacterEquipmentDataDB characterEquipmentDataDB)
    {

        EquipmentDataSO equipmentSo = EquipmentManager.Instance.GetEquipmentData(characterEquipmentDataDB.equipmentId);
        if(equipmentSo == null)
        {
            MyLog.LogError($"null 받음");
            return;
        }
        EquipmentDataSO = equipmentSo;
        Level = characterEquipmentDataDB.equipmentLevel;
        UpdateRuntimeData();
    }

    public bool IsSameEquipment(EquipmentInstance other)
    {
        if(EquipmentDataSO.EquipmentId == other.EquipmentDataSO.EquipmentId && Level == other.Level)
        {
            return true;
        }
        return false;
    }
    public RuntimeEquipmentData GetRuntimeEquipmentData()
    {
        //한번 갱신후, 반환해주자. 
        UpdateRuntimeData();
        return runtimeEqipmentData;
    }

    public EquipmentInstance(EquipmentDataSO data, int lvl)
    {
        EquipmentDataSO = data;
        Level = lvl;
        UpdateRuntimeData();
    }
    private void UpdateRuntimeData()
    {
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
