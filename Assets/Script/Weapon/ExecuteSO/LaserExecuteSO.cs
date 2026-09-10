using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Laser")]
public class LaserExecuteSO : WeaponExecuteSO
{
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        GameObject gameObject = Instantiate(weapon.weaponDataSO.bulletPrefab, weapon.transform.position, weapon.transform.rotation);
        gameObject.SetActive(false);
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.SetDirectionToEnemyPosition(enemyTarget.position);
            projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
            gameObject.SetActive(true);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("총알 스크립트가 없습니다");
        }
        //gameObject.SetActive(true);
    }
}
