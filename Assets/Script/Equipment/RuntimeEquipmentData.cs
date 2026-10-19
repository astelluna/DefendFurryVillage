using UnityEngine;

public class RuntimeEquipmentData
{
    EquipmentDataSO equipmentDataSO;

    public int attack { get; private set; }
    public int criticalpercent { get; private set; }
    public float criticalRatio { get; private set; }
    public float armorPenetrate { get; private set; }

    public void UpdateData(EquipmentDataSO data, int level)
    {
        equipmentDataSO = data;

        attack = equipmentDataSO.stat.attack + (equipmentDataSO.UpgradeAttackValue * level);
        criticalpercent = equipmentDataSO.stat.criticalPercent + (equipmentDataSO.UpgradeCriticalRate * level);
        criticalRatio = equipmentDataSO.stat.criticalDamageRate + (equipmentDataSO.UpgradeCriticalRatio * level);
        armorPenetrate = equipmentDataSO.stat.armorPenetration + (equipmentDataSO.UpgradeArmorPenetrate * level);
    }
}
