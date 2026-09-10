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
        GameObject gameObject = Instantiate(weapon.weaponDataSO.bulletPrefab, weapon.transform.position, weapon.transform.rotation);
        gameObject.SetActive(false);
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
            projectile.SetDirectionToEnemyPosition(enemyTarget.position);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("총알 스크립트가 없습니다");
        }
        gameObject.SetActive(true);
    }
}
