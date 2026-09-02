using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //상속받으면 해야할거 -> AttackRate, Range, attackCoolTime, Prefab을 등록해야함


    //[SerializeField] protected GameObject weaponPrefab;
    [SerializeField] protected WeaponDataSO weaponDataSO;
    //[SerializeField] protected float weaponAttackRate;
    //[SerializeField] protected float weaponRange;
    protected Coroutine shootCor;
    
    protected float nextAttackTime;

    protected PlayerStat playerStat;

    //TODO. 추후 몬스터 HP 추가하고, 공격력 추가할때 넣자 
    public void DamageCalc()
    {
        //TODO. 추후 총알관통, 크리티컬도 추가해보자.
        //최종 데미지를 계산해서 보낸다.
        //총알을 발사할때마다? weapon클래스에서 총알의 데미지, 관통을 계산해야할듯.

        //playerStat.attack;
    }



    protected virtual void OnDestroy()
    {
        if(shootCor != null)
        {
            StopCoroutine(shootCor);
            shootCor = null;
        }
    }


    //public void SetWeaponInfo(PlayerStat stat, float attackRate, float range)
    public void SetWeaponInfo(PlayerStat stat)
    {
        playerStat = stat;

        if (shootCor == null)
        {
            shootCor = StartCoroutine(ShootWeapon());
        }
        SetWeaponCoolTime();
    }

    protected void SetWeaponCoolTime()
    {
        nextAttackTime = Time.time + weaponDataSO.baseAttackRate;
    }

    //무기 공격 동작 코루틴은 자식이 알아서 하도록 하자.
    protected abstract IEnumerator ShootWeapon();

}
