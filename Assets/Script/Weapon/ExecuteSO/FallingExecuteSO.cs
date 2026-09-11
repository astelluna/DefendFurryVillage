using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Falling")]
public class FallingExecuteSO : WeaponExecuteSO
{
    [SerializeField] float basicExplodeRadius = 3f;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //Falling.cs 씀
        Projectile projectile = Instantiate(weapon.projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        float explodeRadius = basicExplodeRadius + weapon.runtimeData.bulletScale;
        projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
        projectile.SetMoveTartgetPos(enemyTarget.position);
        projectile.SetExplodeRadius(explodeRadius);
        projectile.Shoot();
    }
}
