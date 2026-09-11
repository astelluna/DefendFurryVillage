using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_BasicGun")]
public class BasicGunExecuteSO : WeaponExecuteSO
{
    protected override Transform GetTarget(List<Transform> enemyTransformList, Transform fixedTarget, int index)
    {
        return fixedTarget;
    }

    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        Projectile projectile = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);

        projectile.gameObject.SetActive(false);
        projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
        projectile.SetDirectionToEnemyPosition(enemyTarget.position);
        projectile.Shoot();
        projectile.gameObject.SetActive(true);
        
    }
}
