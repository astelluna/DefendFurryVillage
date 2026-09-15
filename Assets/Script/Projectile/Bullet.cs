using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : Projectile
{
    //[SerializeField] LayerMask enemyLayer;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] Rigidbody rb;

    //TODO. 원래는 외부에서 속도를 받아야하긴하는데. 그냥 고정 속도로 해보자. 그게 나을듯
    float bulletSpeed = 5;

    bool isTriggered = false; //두개 이상 객체에 동시에 충돌해서 추가

    bool canMove = false;

    //지금 해볼것 -> 관통력을 추가해서 몇개이상 관통이 가능하게 할건지 체크해야함.
    //int maxPierceCount = 1;


    //int damage;





    protected override void OnEnable()
    {
        base.OnEnable();
        isTriggered = false;
    }
    private void OnDisable()
    {
        canMove = false;
    }

    private void FixedUpdate()
    {
        //물리적으로 앞으로 이동한다.
        if(canMove)
        {
            Move();
        }
        
    }

    


    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if ((TargetManager.Instance.EnemyLayer.value & (1 << other.gameObject.layer)) == 0) return;

        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {

            //TODO. 추후 오브젝트풀 반환시켜야하고, 바로 죽는게 아닌 데미지 처리하고 죽으면 반환해야함
            //TODO. 적 총알 충돌 처리 -> 추후에는 데미지 달게 할거임

            //일단 n초후에 데미지 받는 거를 테스트 해보자
            //foreach (AugmentSpecialDataSO item in augmentSpecialDataSOList)
            //{
            //    IOnHit onHit = item as IOnHit;
            //    if (onHit != null)
            //    {
            //        onHit.OnHit(weaponRuntimeData, enemy);
            //    }
            //}
            //ApplyAugmentHit(enemy);
            //DamageInfo damageInfo =  MakeDamageInfo(enemy);
            //int calcDamage = DamageManager.Instance.CalcDamage(damageInfo);
            //enemy.TakeDamage(calcDamage);

            //적에 데미지 적용 
            ProcessHit(enemy);
            //총알 관통력이 전부 소모되었으면 총알 없앰
            curPierceCount++;
            if (curPierceCount >= weaponRuntimeData.maxPierce)
            {
                isTriggered = true;
                DestroySelf();
            }
            
        }
    }
    

    public override void Shoot()
    {
        canMove = true;
        Invoke(nameof(DestroySelf), lifeTime);
    }

    protected override void Move()
    {
        rb.MovePosition(transform.position + transform.forward * bulletSpeed * Time.fixedDeltaTime);
    }

    protected override void Init()
    {
        //스케일 설정
        Vector3 scale = Vector3.one;
        float scaleRatio = scale.x * weaponRuntimeData.bulletScale;
        scale *= scaleRatio;
        transform.localScale = scale;
    }

    protected override void DestroySelfSpecial()
    {
        Init();
        //Destroy(this.gameObject);
    }
}
