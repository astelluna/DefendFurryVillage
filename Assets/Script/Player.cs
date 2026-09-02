using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    //일단 플레이어에게 필요한거 정의를 다 해보자
    //TODO. (일단 직렬화 시킴. 추후에는 SO로 캐릭터 데이터 받아서 하거나. 그런식으로 처리해보자)
    [SerializeField] int attack = 10;
    [SerializeField] float attackRate = 1f; //공격 속도
    [SerializeField] float criticalRate = 0; //크리 확률
    [SerializeField] float criticalDamageRate = 2; //크리 데미지 배율 (2배)
    [SerializeField] float armorPenetration = 0; //방어 관통률
    [SerializeField] float weaponRange = 5f; //무기 사거리
    [SerializeField] float specialWeaponRange = 5f; //특수무기 사거리
    [SerializeField] float specialAttackRate = 5f; //공격 속도


    //이게 일반 공격, 특수 공격이 따로 있긴한데 일단 일반 공격만 생각하자
    //#2. Weapon 클래스 분리 예정
    //TODO. 일반 무기 발사 시스템을 따로 넣을까? 일단 추후에 클래스로 분리하고, 여기다가 정의해보자
    //TODO. 추후에는 그냥 발사가 아니라 타겟잡힌거에다가 발사시키자.
    [SerializeField] Bullet bulletPrefab;
    float weaponAttackRate; //Player의 공격속도를 weapon 클래스에 넣자.
    WaitForSeconds delay;
    Coroutine shootCor;

    //특수 공격
    [SerializeField] LaserWeapon laserWeapon;
    [SerializeField] ShootGunWeapon shootGunWeapon;

    [SerializeField] GrowingLaser LaserPrefab;
    float specialWeaponAttackRate;
    WaitForSeconds specialDealy;
    Coroutine specialShootCor;

    IEnumerator Shoot()
    {
        while(true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함
            Transform enemyTarget = TargetManager.Instance.GetEnemyTarget(weaponRange);
            if(enemyTarget != null)
            {
                Bullet bullet = Instantiate(bulletPrefab, transform);
                bullet.SetDirection(enemyTarget.position);
            }
            
            yield return delay;
        }
        
    }

    
    private void Awake()
    {
        weaponAttackRate = attackRate;
        delay = new WaitForSeconds(weaponAttackRate);

    }
    private void Start()
    {
        //TODO. 추후에는 Weapon 클래스에서 코루틴 실행시키자
        if(shootCor == null)
        {
            shootCor = StartCoroutine(Shoot());
        }
        if(specialShootCor == null)
        {
            //specialShootCor = StartCoroutine(SpecialShoot());
        }
        //TODO. 특수무기 등록 나중에는 1개씩만 등록하게 해야함. Weapon??
        if (laserWeapon != null)
        {
            laserWeapon.SetWeaponInfo(specialAttackRate, specialWeaponRange);
        }
        if (shootGunWeapon != null)
        {
            shootGunWeapon.SetWeaponInfo(specialAttackRate, specialWeaponRange);
        }
    }


    //TODO. 추후 데미지 계산
    public float CalcDamage( int armor)
    {
        //현재 공격이 치명타인지 확인.
        bool isCritical = false;
        float damage = attack;
        float percent = Random.Range(0, 100);
        if(percent >= criticalRate)
        {
            //크리
            isCritical = true;
        }

        if(isCritical)
        {
            damage *= criticalDamageRate;
        }

        //아머를 고려해서 최종 데미지 계산
        float result = damage * (100 / (100 + armor));
        return result;

    }

    
}
