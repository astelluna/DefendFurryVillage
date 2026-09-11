using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Missile")]
public class MissileExecuteSO : WeaponExecuteSO
{
    [SerializeField] float basicExplodeRadius = 3f;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //Missile.cs 씀
        Projectile projectile = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        float explodeRadius = basicExplodeRadius + weapon.runtimeData.bulletScale;
        projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
        projectile.SetTargetPos(enemyTarget.position);
        projectile.SetExplodeRadius(explodeRadius);
        projectile.Shoot();
    }
}
