using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Missile")]
public class MissileExecuteSO : WeaponExecuteSO
{
    [SerializeField] float basicExplodeRadius = 3f;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon, AudioClip destroySound)
    {
        //Missile.cs 씀
        //Projectile projectile = Instantiate(weapon.ProjectilePrefab, weapon.transform.position, weapon.transform.rotation);
        Projectile projectile = ProjectileSpawner.Instance.GetPrefabByPool(weapon.transform, weapon.ProjectilePrefab);
        float explodeRadius = basicExplodeRadius + weapon.runtimeData.bulletScale;
        projectile.SetProjectileDataAndInit(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList), destroySound);
        projectile.SetTargetPos(enemyTarget.position);
        projectile.SetExplodeRadius(explodeRadius);
        projectile.Shoot();
    }
}
