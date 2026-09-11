using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Shuriken")]
public class ShurikenExecuteSO : WeaponExecuteSO
{
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //수리검은 기본적으로 자기 자신이 회전을 계속 한다. 그런 상태에서 
        Vector3 direction = enemyTarget.position - weapon.transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Projectile projectile = Instantiate(weapon.projectilePrefab, weapon.transform.position, rotation);
        projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
        projectile.SetTargetPos(enemyTarget.position);
        projectile.Shoot();
    }
}
