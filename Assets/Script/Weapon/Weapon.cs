using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected GameObject weaponPrefab;
    protected float weaponAttackRate;
    protected float weaponRange;
    protected WaitForSeconds delay;
    protected Coroutine shootCor;
    

    protected float nextAttackTime;


    protected virtual void OnDestroy()
    {
        if(shootCor != null)
        {
            StopCoroutine(shootCor);
            shootCor = null;
        }
    }


    public void SetWeaponInfo(float attackRate, float range)
    {
        weaponAttackRate = attackRate;
        weaponRange = range;
        delay = new WaitForSeconds(weaponAttackRate);

        if (shootCor == null)
        {
            shootCor = StartCoroutine(ShootWeapon());
        }
        SetWeaponCoolTime();
    }

    protected void SetWeaponCoolTime()
    {
        nextAttackTime = Time.time + weaponAttackRate;
    }

    //무기 공격 동작 코루틴은 자식이 알아서 하도록 하자.
    protected abstract IEnumerator ShootWeapon();

}
