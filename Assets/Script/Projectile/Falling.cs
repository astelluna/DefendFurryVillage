using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Falling : Projectile
{
    float yPos = 10f;
    float fallingSpeed = 6f;

    bool canMove = false;
    [SerializeField] Rigidbody rb;

    //최대 100개를 감지하도록 하자
    Collider[] colliders = new Collider[100];

    public override void Shoot()
    {
        //투사체가 일정 높이에서 시작해야함
        Vector3 pos = transform.position;
        pos.y += yPos;
        transform.position = pos;
        canMove = true;
    }

    protected override void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    protected override void Init()
    {
        //throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & TargetManager.Instance.GroundLayer.value) == 0) return;
        //바닥에 부딫힌 위치에서 범위를 계산해서 데미지를 주면 된다. 
        //TODO. 추후 바닥에 부딫히면 충격파 이펙트 넣어야 함.
        var k = Physics.OverlapSphereNonAlloc(transform.position, explodeRadius, colliders);
        for(int i=0; i< k; i++)
        {
            if (colliders[i].TryGetComponent(out Enemy enemy))
            {
                DamageInfo damageInfo = MakeDamageInfo(enemy);
                int calcDamage = DamageManager.Instance.CalcDamage(damageInfo);
                enemy.TakeDamage(calcDamage);
            }
        }
        DestroySelf();
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        //y축으로 쭉 내려가면 됨.
        if(canMove)
        {
            Vector3 pos = new Vector3(rb.position.x, rb.position.y + (-fallingSpeed * Time.fixedDeltaTime), rb.position.z);
            rb.MovePosition(pos);
        }
    }
}
