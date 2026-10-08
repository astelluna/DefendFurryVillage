using System;
using UnityEngine;

public class CastleWall : MonoBehaviour
{
    [SerializeField] int hp = 1500;
    [SerializeField] int defense = 0;

    DefenserInfo defenserInfo;

    public event Action CastleDestroyRequest;

    //public void TakeDamage(int dmg)
    public void TakeDamage(AttackerInfo attackerInfo)
    {
        if(defenserInfo == null)
        {
            defenserInfo = new DefenserInfo();
            defenserInfo.SetDefenserInfo(defense);
        }
        int dmg = DamageManager.Instance.CalcDamage(attackerInfo, defenserInfo);
        hp -= dmg;
        if(hp <= 0)
        {
            hp = 0;
            CastleDestroyRequest?.Invoke();
        }
        //MyLog.Log($"HP {hp}");
    }

}
