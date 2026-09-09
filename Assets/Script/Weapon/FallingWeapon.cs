using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWeapon : Weapon
{
    //위에서 폭탄, 돌, 용 등등 어떤 물체가 떨어지는 그런 무기 공격들
    [SerializeField] float basicExplodeRadius = 3f;
    protected override IEnumerator ShootWeapon()
    {
        //총알 발사
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            int attackCount = runtimeData.plusAttackCount;
            List<Transform> enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange, attackCount);

            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null && enemyTarget.Count > 0)
            {
                //발사를 하는데 공격횟수에 따라서 여러번 발사를 해버려야함
                Transform selectedTrf = enemyTarget[0];
                for (int i = 0; i < attackCount; i++)
                {
                    //최대한 다른 적을 공격한다고 가정하자.
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
        //Falling.cs 씀
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            float explodeRadius = basicExplodeRadius + runtimeData.bulletScale;
            projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
            projectile.SetMoveTartgetPos(enemyTarget.position);
            projectile.SetExplodeRadius(explodeRadius);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("떨어지는 투사체 찾기 실패");
        }
        SetWeaponCoolTime();
    }
}
