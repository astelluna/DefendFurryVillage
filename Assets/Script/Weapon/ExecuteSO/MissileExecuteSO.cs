using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Missile")]
public class MissileExecuteSO : WeaponExecuteSO
{
    [SerializeField] float basicExplodeRadius = 3f;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //그냥 미사일 쏘자
        GameObject gameObject = Instantiate(weapon.weaponDataSO.bulletPrefab, weapon.transform.position, weapon.transform.rotation);
        //Missile.cs 씀
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            float explodeRadius = basicExplodeRadius + weapon.runtimeData.bulletScale;
            projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
            projectile.SetTargetPos(enemyTarget.position);
            projectile.SetExplodeRadius(explodeRadius);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("미사일 찾기 실패");
        }
    }
}
