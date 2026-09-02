using System.Collections;
using UnityEngine;

public class MissileWeapon : Weapon
{
    //필요한것. Missile라는 발사체를 하나 만들자. 그 미사일은 알아서 발사를 하고 해당 지점에 도달하면 죽도록 하는거임.
    [SerializeField] float missileRadius = 3f;

    protected override IEnumerator ShootWeapon()
    {
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            Transform enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null)
            {
                //스페셜 공격중 샷건 쓴다고 생각해보자.
                ShootBullet(enemyTarget);
            }
            yield return null;

        }
    }

    private void ShootBullet(Transform enemyTarget)
    {
        //그냥 미사일 쏘자
        GameObject gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, transform.rotation);
        if (gameObject.TryGetComponent(out Missile missile))
        {
            missile.Shoot(enemyTarget.position, missileRadius);
        }
        else
        {
            Debug.Log("미사일 찾기 실패");
        }
        SetWeaponCoolTime();
    }

}
