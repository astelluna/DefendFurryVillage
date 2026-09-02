using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/UpgradeData")]
public class WeaponUpgradeDataSO : ScriptableObject
{
    public string upgradeId;
    public float attackRateBonus;
    public float rangeBonus;
    public int damageBonus;
    public int addbulletHitBonus; //총알 관통력 보너스
    public int maxStack = 5;
}
