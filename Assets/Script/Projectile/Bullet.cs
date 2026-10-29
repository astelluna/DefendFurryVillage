using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : Projectile
{
    //[SerializeField] LayerMask enemyLayer;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] Rigidbody rb;

    // 고정 속도로
    float bulletSpeed = 8;
    bool isTriggered = false; //두개 이상 객체에 동시에 충돌해서 추가
    bool canMove = false;

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
