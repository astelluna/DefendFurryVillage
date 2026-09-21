using UnityEngine;

public class CastleWall : MonoBehaviour
{
    [SerializeField] int hp = 1500;
    [SerializeField] int defense = 0;

    //public void TakeDamage(int dmg)
    public void TakeDamage(DamageInfo damageInfo)
    {
        damageInfo.SetDamageInfo(damageInfo.attackerAttack, 0, 0, damageInfo.attackerArmorPenetrate, defense);
        int dmg = DamageManager.Instance.CalcDamage(damageInfo);
        hp -= dmg;
        //MyLog.Log($"HP {hp}");
    }

}
