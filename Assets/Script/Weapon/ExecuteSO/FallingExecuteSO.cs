using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Execute/WeaponExecute_Falling")]
public class FallingExecuteSO : WeaponExecuteSO
{
    [SerializeField] float basicExplodeRadius = 3f;
    protected override void ShootAt(Transform enemyTarget, Weapon weapon)
    {
        //그냥 미사일 쏘자
        GameObject gameObject = Instantiate(weapon.weaponDataSO.bulletPrefab, weapon.transform.position, weapon.transform.rotation);
        //Falling.cs 씀
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            float explodeRadius = basicExplodeRadius + weapon.runtimeData.bulletScale;
            projectile.SetProjectileData(weapon.runtimeData, new List<AugmentSpecialDataSO>(weapon.AugmentSpecialDataSOList));
            projectile.SetMoveTartgetPos(enemyTarget.position);
            projectile.SetExplodeRadius(explodeRadius);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("떨어지는 투사체 찾기 실패");
        }
    }
}
