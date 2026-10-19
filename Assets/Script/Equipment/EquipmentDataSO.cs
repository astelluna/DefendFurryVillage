using UnityEngine;
using UnityEngine.Rendering;

public enum eEquipmentType
{
    //이름 절대 바꾸지말것. DB에서 이 이름으로 저장중이다
    Weapon, Armor, Accessories
}

[CreateAssetMenu(menuName = "Equipment/EquipmentData")]
public class EquipmentDataSO : ScriptableObject
{
    [Header("아이템 정보")]
    [field: SerializeField] public string EquipmentId { get; private set; }
    [field: SerializeField] public eEquipmentType EquipmentType { get; private set; }
    [field: SerializeField] public string EquipmentName { get; private set; }
    [field: SerializeField] public Sprite EquipmentImage { get; private set; }
    //캐릭터 스텟을 몇 올리는지 나옴
    [Header("아이템 기본 스텟")]
    [field: SerializeField] public CharacterStat stat { get; private set; }
    [Header("아이템 업그레이드 시 오르는 스텟")]
    [field: SerializeField] public int UpgradeAttackValue { get; private set; } = 1;
    [field: SerializeField] public int UpgradeCriticalRate { get; private set; } = 1;
    [field: SerializeField] public float UpgradeCriticalRatio { get; private set; } = 1f;
    [field: SerializeField] public float UpgradeArmorPenetrate { get; private set; } = 1f;
}
