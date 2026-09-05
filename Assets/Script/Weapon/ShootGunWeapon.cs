using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGunWeapon : Weapon
{
    ///private Transform enemyTarget; //TODO. 이거 매니저에 옮겨서 하거나 해야할듯. 
    //TODO. 샷건도 Set으로 뭔가 조절할수있게 해야함. 증강형식으로
    int maxBulletHitCount = 5;
    int bulletCount = 5;
    float spreadAngle = 90;

    protected override IEnumerator ShootWeapon()
    {
        while (true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            int attackCount = runtimeData.plusAttackCount;
            List<Transform> enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponDataSO.baseRange,attackCount);
            //무기 쿨타임이 지났고, 타겟을 확인헀다면 발사
            if (Time.time >= nextAttackTime && enemyTarget != null && enemyTarget.Count > 0)
            {
                //스페셜 공격중 샷건 쓴다고 생각해보자.
                Transform selectedTrf = enemyTarget[0];
                for (int i = 0; i < attackCount; i++)
                {
                    //샷건은 무조건 0번째 적을 노려보자.
                    if (selectedTrf == null)
                    {
                        selectedTrf = FindExistTransform(enemyTarget);
                        if (selectedTrf == null)
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
    //테스트용 현재 선택된 증강 리스트
    [SerializeField] List<AugmentDataSO> testAugmentDataSOList; //테스트용
    
    
    private void Awake()
    {
        augmentDataList = testAugmentDataSOList; //테스트용
        UpdateWeaponData(); //테스트용
    }

    


    private void ShootBullet(Transform enemyTarget)
    {
        float startAngle = -spreadAngle / 2f;
        float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f; // 30

        //적 방향 계산
        Vector3 direction = enemyTarget.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Quaternion rotation = targetRotation * Quaternion.Euler(0f, angle, 0f);
            var gameObject = Instantiate(weaponDataSO.bulletPrefab, transform.position, rotation);

            //if (gameObject.TryGetComponent(out Bullet bullet))
            //발사체로 Bullet.cs쓴다
            if (gameObject.TryGetComponent(out Projectile projectile))
            {
                projectile.SetProjectileData(runtimeData, new List<AugmentSpecialDataSO>(augmentSpecialDataSOList));
            }

        }
        SetWeaponCoolTime();
    }
}
