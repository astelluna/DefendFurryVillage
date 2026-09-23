using UnityEngine;
using UnityEngine.Rendering;
[CreateAssetMenu(menuName = "Equipment/EquipmentData")]
public class EquipmentDataSO : ScriptableObject
{
    [field: SerializeField] public string EquipmentName { get; private set; }
    [field: SerializeField] public Sprite EquipmentImage { get; private set; }
    [field: SerializeField] public int Attack { get; private set; }
    [field: SerializeField] public float CriticalRate { get; private set; }
    [field: SerializeField] public float CriticalRatio { get; private set; }
    [field: SerializeField] public float ArmorPenetrate { get; private set; }

    [field: SerializeField] public int UpgradeAttackValue { get; private set; } = 1;
    [field: SerializeField] public float UpgradeCriticalRate { get; private set; } = 1f;
    [field: SerializeField] public float UpgradeCriticalRatio { get; private set; } = 1f;
    [field: SerializeField] public float UpgradeArmorPenetrate { get; private set; } = 1f;


    
}
