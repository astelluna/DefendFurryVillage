using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{

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
                //스페셜 공격중 레이저를 쓴다고 생각해보자. -> GrowingLaser.cs 씀
                for(int i=0; i< attackCount; i++)
                {
                    //레이저는 발사할때 무조건 다른 적을 쏜다고 가정하자. 
                    int idx = i % enemyTarget.Count;
                    Transform trf = enemyTarget[idx];
                    if(trf == null)
                    {
                        trf = FindExistTransform(enemyTarget);
                        if(trf == null)
                        {
                            yield return continuousFireDelay;
                            continue;
                        }
                    }

                    Projectile projectile = Instantiate(weaponDataSO.bulletPrefab, transform).GetComponent<Projectile>();
                    projectile.SetDirectionToEnemyPosition(trf.position);
                    projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
                    projectile.Shoot();
                    yield return continuousFireDelay;
                }
                


                SetWeaponCoolTime();

            }
            yield return null;
        }
    }
}
