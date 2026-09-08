using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    //기본 데이터
    protected WeaponRuntimeData weaponRuntimeData;
    //무기에서 총알 쏘기전에 설정한. 특정 행동들이 필요한 것들을 전부 모아둔 리스트
    protected List<AugmentSpecialDataSO> augmentSpecialDataSOList;
    //총알에 있는 상태이상 효과들을 저장한 리스트
    protected List<IStatusEffect> statusEffectList; 


    protected int curPierceCount = 0;

    protected virtual void OnEnable()
    {
        curPierceCount = 0;
    }

    //설정
    protected Vector3 targetPos;
    protected float explodeRadius;

    
    public virtual void SetTargetPos(Vector3 pos) => targetPos = pos; //타겟 위치로 날아가야할때 사용
    public virtual void SetExplodeRadius(float amount) => explodeRadius = amount; //폭발 범위가 필요할때 사용


    public abstract void Shoot();

    protected abstract void Move();

    protected abstract void Init();
    public virtual void SetProjectileData(WeaponRuntimeData data, List<AugmentSpecialDataSO> specialDataSOs)
    {
        weaponRuntimeData = data;
        //null이면 빈거 넣음
        augmentSpecialDataSOList = (specialDataSOs == null) ? new List<AugmentSpecialDataSO>() : specialDataSOs;
    }

    //TODO.//추후 여기서 그냥 오브젝트풀에 반환하도록 구현해둘수있을듯?
    protected abstract void DestroySelf();


    protected virtual DamageInfo MakeDamageInfo(Enemy enemy)
    {
        int enemyArmor = enemy.GetEnemyAromr();
        DamageInfo damageInfo = new DamageInfo();
        damageInfo.SetDamageInfo(weaponRuntimeData.attackdamage, weaponRuntimeData.criticalPercent, weaponRuntimeData.criticalDamgeRate, weaponRuntimeData.armorPenetrate, enemyArmor);
        return damageInfo;
    }


    //총알 날아가는 방향 지정
    public virtual void SetDirectionToEnemyPosition(Vector3 dir)
    {
        //몬스터 방향 위치으로 날아가야하면 이렇게
        this.transform.LookAt(dir);
    }
    public virtual void SetDirectionToDirection(Vector3 enemyPos, Vector3 dir)
    {
        //앞쪽, 뒤쪽,왼쪽 이런식으로 특정 방향을 봐야하는거면 이렇게
        transform.position = enemyPos;
        this.transform.LookAt(transform.position + dir);
        Init();
    }
}
