using System;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileDirectionSetter
{
    void Apply(Projectile projectile);
}

public struct ProjectileSpawnContext
{
    public WeaponRuntimeData WeaponData;
    public List<AugmentSpecialDataSO> augmentDataList;
    public IProjectileDirectionSetter DirectionSetter;
    public Vector3? TargetPos;
    public float? ExplodeRadius;
}

public abstract class Projectile : MonoBehaviour
{
    //기본 데이터
    protected WeaponRuntimeData weaponRuntimeData;
    //무기에서 총알 쏘기전에 설정한. 특정 행동들이 필요한 것들을 전부 모아둔 리스트
    protected List<AugmentSpecialDataSO> augmentSpecialDataSOList;
    //총알에 있는 상태이상 효과들을 저장한 리스트
    protected List<IStatusEffect> statusEffectList; 


    protected int curPierceCount = 0;

    //오브젝트 풀
    ObjPool<Projectile> projectilePool;
    public event Action<Projectile> OnProjectileDestroy;

    //이펙트
    [SerializeField] EffectScript projectileEffect;

    //총알 - 공격자 정보 
    AttackerInfo projectileAttackerInfo = new AttackerInfo();

    public void SetObjectPool(ObjPool<Projectile> pool)
    {
        projectilePool = pool;
    }

    protected virtual void OnEnable()
    {
        curPierceCount = 0;
    }

    //설정
    protected Vector3 targetPos;
    protected float explodeRadius;

    public virtual void SetMoveTartgetPos(Vector3 pos) => transform.position = pos; //위에서 떨어지는 공격을 위해 자신의 위치를 타겟 바로위로 지정
    public virtual void SetTargetPos(Vector3 pos) => targetPos = pos; //타겟 위치로 날아가야할때 사용
    public virtual void SetExplodeRadius(float amount) => explodeRadius = amount; //폭발 범위가 필요할때 사용
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
    }
    public virtual void SetDirectionRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public virtual void SetProjectileDataAndInit(WeaponRuntimeData data, List<AugmentSpecialDataSO> specialDataSOs)
    {
        weaponRuntimeData = data;
        //null이면 빈거 넣음
        augmentSpecialDataSOList = (specialDataSOs == null) ? new List<AugmentSpecialDataSO>() : specialDataSOs;
        //총알 공격자 정보 셋팅
        projectileAttackerInfo.SetAttackerInfo(weaponRuntimeData.attackdamage, weaponRuntimeData.criticalPercent, weaponRuntimeData.criticalDamgeRate, weaponRuntimeData.armorPenetrate);
        //총알 크기를 weaponRunTimeData만큼 키우거나 그래야한다면 Init에서 하자
        Init();
    }

    protected void ProcessHit(Enemy enemy)
    {
        //특수  공격을 적에게 적용 (화상, 넉백.. 등등)
        ApplyAugmentHit(enemy);
        //데미지를 적에게 적용
        int calcDamage = DamageManager.Instance.CalcDamage(projectileAttackerInfo, enemy.EnemyDefenserInfo);
        enemy.TakeDamage(calcDamage);
    }

    protected void ApplyAugmentHit(Enemy enemy)
    {
        foreach (AugmentSpecialDataSO item in augmentSpecialDataSOList)
        {
            IOnHit onHit = item as IOnHit;
            if (onHit != null)
            {
                onHit.OnHit(weaponRuntimeData, enemy);
            }
        }
    }


    public abstract void Shoot();

    protected abstract void Move();

    protected abstract void Init();
    protected virtual void DestroySelf()
    {
        //이펙트를 표시
        if(projectileEffect != null)
        {
            EffectManager.Instance.Playfbx(projectileEffect, this.transform);
        }

        //파괴 이벤트를 보내고 풀도 스스로 반환시킴
        OnProjectileDestroy?.Invoke(this);
        
        //총알마다 따로 처리해야할 작업들은 여기서 한다
        DestroySelfSpecial();

        //자기자신 스스로 풀에 처리 
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        projectilePool.ReturnObject(this);
    }
    protected abstract void DestroySelfSpecial();
}
