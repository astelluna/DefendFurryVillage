using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct WeaponRuntimeData
{
    public int attackdamage;
    public float attackRate;
    public float bulletScale;
    public int plusAttackCount;
    public int maxPierce;
}
public abstract class Weapon : MonoBehaviour
{
    //런타임 데이터
    protected WeaponRuntimeData runtimeData = new WeaponRuntimeData();
    protected List<AugmentSpecialDataSO> augmentSpecialDataSOList = new List<AugmentSpecialDataSO>();

    //발사 횟수가 2회이상일때 딜레이를 위해서 추가
    protected float shootDelay = 0.5f;
    protected WaitForSeconds continuousFireDelay;


    //증강이 선택되면 현재 무기의 런타임 정보를 갱신한다.
    protected void UpdateWeaponData()
    {
        float damagePercent = 0;
        float ratePercent = 0;
        float bulletSizePercent = 0;
        int attackCount = 0;
        int pierceCount = 0;

        foreach (AugmentDataSO item in augmentDataList)
        {
            damagePercent += item.addAttackDamage;
            ratePercent += item.reduceAttackRate;
            bulletSizePercent += item.addProjectileScale;
            attackCount += item.addAttackCount;
            pierceCount += item.addPierceCount;

            if (item.AugmentSpecialDataSOList != null && item.AugmentSpecialDataSOList.Count > 0)
            {
                augmentSpecialDataSOList.AddRange(item.AugmentSpecialDataSOList);
            }
        }

        //runtimeData = new WeaponRuntimeData();
        runtimeData.attackdamage = weaponDataSO.baseDamage + (int)(weaponDataSO.baseDamage * damagePercent);
        runtimeData.attackRate = weaponDataSO.baseAttackRate + (weaponDataSO.baseAttackRate * ratePercent);
        runtimeData.bulletScale = 1f + (1f * bulletSizePercent);
        runtimeData.plusAttackCount = attackCount;
        runtimeData.maxPierce = 1 + pierceCount;

        //후처리
        if (runtimeData.attackdamage < 1) runtimeData.attackdamage = 1;
        runtimeData.attackRate = Mathf.Round(runtimeData.attackRate * 100f) / 100f;
        if (runtimeData.bulletScale < 0.1) runtimeData.bulletScale = 0.1f;
    }



    //[SerializeField] protected GameObject weaponPrefab;
    [SerializeField] protected WeaponDataSO weaponDataSO;
    //[SerializeField] protected float weaponAttackRate;
    //[SerializeField] protected float weaponRange;
    protected Coroutine shootCor;
    
    protected float nextAttackTime;

    protected PlayerStat playerStat;

    //현재 증강 리스트
    protected List<AugmentDataSO> augmentDataList = new List<AugmentDataSO>();


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
        continuousFireDelay = new WaitForSeconds(shootDelay);
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
    protected virtual Transform FindExistTransform(List<Transform> enemyTransform)
    {
        foreach (var item in enemyTransform)
        {
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }
}
