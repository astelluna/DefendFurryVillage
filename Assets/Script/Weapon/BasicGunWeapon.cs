using System.Collections;
using UnityEngine;

public class BasicGunWeapon : Weapon
{
    protected override IEnumerator ShootWeapon()
    {
        //총알 발사
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            Transform enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange);

            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null)
            {
                //발사
                ShootBullet(enemyTarget);
            }
            yield return null;
        }
    }
    private void ShootBullet(Transform enemyTarget)
    {
        GameObject gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, transform.rotation);
        if (gameObject.TryGetComponent(out Bullet bullet))
        {
            bullet.SetDirection(enemyTarget.position);
        }
        else
        {
            Debug.LogError("총알 스크립트가 없습니다");
        }
        SetWeaponCoolTime();
    }
}
