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
    [SerializeField] float weaponRange; //무기 사거리

    //이게 일반 공격, 특수 공격이 따로 있긴한데 일단 일반 공격만 생각하자
    //#2. Weapon 클래스 분리 예정
    //TODO. 일반 무기 발사 시스템을 따로 넣을까? 일단 추후에 클래스로 분리하고, 여기다가 정의해보자
    //TODO. 추후에는 그냥 발사가 아니라 타겟잡힌거에다가 발사시키자.
    [SerializeField] Bullet bulletPrefab;
    float weaponAttackRate; //Player의 공격속도를 weapon 클래스에 넣자.
    WaitForSeconds delay;
    Coroutine shootCor;

    IEnumerator Shoot()
    {
        while(true)
        {
            //TODO. 추후 오브젝트풀로 바꿔야함

            if(enemyTarget != null)
            {
                Bullet bullet = Instantiate(bulletPrefab, transform);
                bullet.SetDirection(enemyTarget.position);
            }
            
            yield return delay;
        }
        
    }
    //#3. Target 시스템 분리 예정
    //플레이어가 총알을 쏠때 어떤 타겟 방향으로 총알을 쏴야하는지 결정한다.
    [SerializeField] Transform basicPosition;
    [SerializeField] float radius;
    [SerializeField] LayerMask enemyLayer;
    Collider[] result = new Collider[30];
    Transform enemyTarget;
    //타겟 찾는거는 제일 가까운거 찾아두고, 무기 사거리에 따라서 알아서 공격하도록 하자

    private Transform FindEnemyTarget()
    {
        //원형으로 체크를 해볼까? Enemy 타겟을 찾는거임
        //Collider[] colliders = Physics.OverlapBoxNonAlloc(,)

        //var k = Physics.OverlapBoxNonAlloc(basicPosition.position, radius, result, enemyLayer);
        var enemyCount = Physics.OverlapSphereNonAlloc(basicPosition.position, radius, result, enemyLayer);
        if (enemyCount == 0)
        {
            enemyTarget = null;
            return null;
        }
        float nearestDistance = float.MaxValue;
        Transform enemyTransform = null;
        for(int i=0; i< enemyCount; i++)
        {
            float distance = Vector3.Distance(basicPosition.position, result[i].transform.position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                enemyTransform = result[i].transform;
            }
        }
        enemyTarget = enemyTransform;
        return enemyTransform;
    }

    private void Update()
    {
        var k =  FindEnemyTarget();
        Debug.Log(k);
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
