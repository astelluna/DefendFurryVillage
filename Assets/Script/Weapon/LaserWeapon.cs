using System.Collections;
using UnityEngine;

public class LaserWeapon : Weapon
{

    protected override IEnumerator ShootWeapon()
    {
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            Transform enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponRange);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null)
            {
                //스페셜 공격중 레이저를 쓴다고 생각해보자.
                GrowingLaser laser = Instantiate(weaponPrefab, transform).GetComponent<GrowingLaser>();
                laser.SetDirection(enemyTarget.position);
                SetWeaponCoolTime();
            }
            yield return null;
        }
    }
}
