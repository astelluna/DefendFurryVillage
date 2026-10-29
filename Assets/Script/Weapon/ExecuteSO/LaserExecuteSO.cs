using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Laser")]
public class LaserExecuteSO : WeaponExecuteSO
{
    protected override void ShootAt(Transform enemyTarget, Weapon weapon, AudioClip destroySound)
    {
        //GameObject gameObject = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        //Projectile projectile = Instantiate(weapon.ProjectilePrefab, weapon.transform.position, weapon.transform.rotation);
        Projectile projectile = ProjectileSpawner.Instance.GetPrefabByPool(weapon.transform, weapon.ProjectilePrefab);
        projectile.gameObject.SetActive(false);
        projectile.SetDirectionToEnemyPosition(enemyTarget.position);
        projectile.SetProjectileDataAndInit(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList), destroySound);
        projectile.gameObject.SetActive(true);
        projectile.Shoot();
    }
}
