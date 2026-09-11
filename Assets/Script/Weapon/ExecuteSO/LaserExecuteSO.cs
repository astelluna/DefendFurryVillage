using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Laser")]
public class LaserExecuteSO : WeaponExecuteSO
{
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //GameObject gameObject = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        Projectile projectile = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        projectile.gameObject.SetActive(false);
        projectile.SetDirectionToEnemyPosition(enemyTarget.position);
        projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
        projectile.gameObject.SetActive(true);
        projectile.Shoot();
    }
}
