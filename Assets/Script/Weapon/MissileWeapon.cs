using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : Weapon
{
    //필요한것. Missile라는 발사체를 하나 만들자. 그 미사일은 알아서 발사를 하고 해당 지점에 도달하면 죽도록 하는거임.
    [SerializeField] float missileRadius = 3f;
    //테스트용 현재 선택된 증강 리스트
    [SerializeField] List<AugmentDataSO> testAugmentDataSOList; //테스트용


    protected override IEnumerator ShootWeapon()
    {
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            int attackCount = runtimeData.plusAttackCount;
            List<Transform> enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange, attackCount);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null && enemyTarget.Count > 0)
            {
                //스페셜 공격중 샷건 쓴다고 생각해보자.
                for (int i = 0; i < attackCount; i++)
                {
                    //미사일도 최대한 다른 적을 공격한다고 가정하자.
                    int idx = i % enemyTarget.Count;
                    Transform trf = enemyTarget[idx];
                    if (trf == null)
                    {
                        trf = FindExistTransform(enemyTarget);
                        if (trf == null)
                        {
                            yield return continuousFireDelay;
                            continue;
                        }
                    }

                    ShootBullet(trf);
                    yield return continuousFireDelay;
                }
            }
            yield return null;

        }
    }
    

    private void ShootBullet(Transform enemyTarget)
    {
        //그냥 미사일 쏘자
        GameObject gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, transform.rotation);
        //Missile.cs 씀
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
            projectile.SetTargetPos(enemyTarget.position);
            projectile.SetExplodeRadius(missileRadius);
            projectile.Shoot();
        }
        else
        {
            MyLog.Log("미사일 찾기 실패");
        }
        SetWeaponCoolTime();
    }

}
