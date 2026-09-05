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
            Transform enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null)
            {
                //스페셜 공격중 레이저를 쓴다고 생각해보자. -> GrowingLaser.cs 씀
                Projectile projectile = Instantiate(weaponDataSO.bulletPrefab, transform).GetComponent<Projectile>();
                projectile.SetDirectionToEnemyPosition(enemyTarget.position);
                projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
                projectile.Shoot();
                SetWeaponCoolTime();
                //GrowingLaser laser = Instantiate(weaponDataSO.bulletPrefab, transform).GetComponent<GrowingLaser>();
                //laser.SetDirection(enemyTarget.position);
                //SetWeaponCoolTime();
            }
            yield return null;
        }
    }
}
