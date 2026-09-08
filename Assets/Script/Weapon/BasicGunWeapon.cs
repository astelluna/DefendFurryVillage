using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGunWeapon : Weapon
{
    //테스트용 현재 선택된 증강 리스트
    //[SerializeField] List<AugmentDataSO> testAugmentDataSOList; //테스트용


    protected override IEnumerator ShootWeapon()
    {
        //총알 발사
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            int attackCount = runtimeData.plusAttackCount;
            List<Transform> enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange, attackCount);

            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null && enemyTarget.Count > 0)
            {
                //발사를 하는데 공격횟수에 따라서 여러번 발사를 해버려야함
                Transform selectedTrf = enemyTarget[0];
                for (int i=0; i< attackCount; i++)
                {
                    //기본총알은 무조건 0순위 적한테 쏘도록 하자
                    if (selectedTrf == null)
                    {
                        selectedTrf = FindExistTransform(enemyTarget);
                        if(selectedTrf == null)
                        {
                            yield return continuousFireDelay;
                            continue;
                        }
                    }
                    ShootBullet(selectedTrf);
                    yield return continuousFireDelay;
                }

                
            }
            yield return null;
        }
    }
    private void ShootBullet(Transform enemyTarget)
    {
        GameObject gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
        if (gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
            projectile.SetDirectionToEnemyPosition(enemyTarget.position);
            projectile.Shoot();
        }
        else
        {
            MyLog.LogError("총알 스크립트가 없습니다");
        }
        gameObject.SetActive(true);
        SetWeaponCoolTime();
    }
}
