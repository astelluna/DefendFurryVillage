using UnityEngine;

public class RuntimeEquipmentData
{
    EquipmentDataSO equipmentDataSO;

    public int attack { get; private set; }
    public float criticalRate { get; private set; }
    public float criticalRatio { get; private set; }
    public float armorPenetrate { get; private set; }

    public void UpdateData(EquipmentDataSO data, int level)
    {
        equipmentDataSO = data;

        attack = equipmentDataSO.Attack + (equipmentDataSO.UpgradeAttackValue * level);
        criticalRate = equipmentDataSO.CriticalRate + (equipmentDataSO.UpgradeCriticalRate * level);
        criticalRatio = equipmentDataSO.CriticalRatio + (equipmentDataSO.UpgradeCriticalRatio * level);
        armorPenetrate = equipmentDataSO.ArmorPenetrate + (equipmentDataSO.ArmorPenetrate * level);
    }
}
