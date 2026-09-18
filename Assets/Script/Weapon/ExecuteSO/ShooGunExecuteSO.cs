using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_ShootGun")]
public class ShooGunExecuteSO : WeaponExecuteSO
{
    int bulletCount = 5;
    float spreadAngle = 90;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        float startAngle = -spreadAngle / 2f;
        float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f; // 30

        //적 방향 계산
        Vector3 direction = enemyTarget.position - weapon.transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Quaternion rotation = targetRotation * Quaternion.Euler(0f, angle, 0f);
            //발사체로 Bullet.cs쓴다
            //Projectile projectile = Instantiate(weapon.ProjectilePrefab, weapon.transform.position, rotation);
            Projectile projectile = ProjectileSpawner.Instance.GetPrefabByPool(weapon.transform, weapon.ProjectilePrefab);
            projectile.SetProjectileDataAndInit(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
            projectile.SetDirectionRotation(rotation);

            projectile.Shoot();
            

        }
    }
}
