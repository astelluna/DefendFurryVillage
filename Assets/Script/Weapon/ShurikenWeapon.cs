using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenWeapon : Weapon
{
    protected override IEnumerator ShootWeapon()
    {
        while(true)
        {
            int attackCount = runtimeData.plusAttackCount;
            List<Transform> enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange, attackCount);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null && enemyTarget.Count > 0)
            {
                //발사를 하는데 공격횟수에 따라서 여러번 발사를 해버려야함
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
        //수리검은 기본적으로 자기 자신이 회전을 계속 한다. 그런 상태에서 
        Vector3 direction = enemyTarget.position - transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        var gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, rotation);
        if(gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
            projectile.SetTargetPos(enemyTarget.position);
            //StartCoroutine(RotateObject(projectile));
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("수리검 찾기 실패");
        }
        SetWeaponCoolTime();
    }


}
