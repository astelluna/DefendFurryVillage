using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shuriken : Projectile
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float rotateSpeed = 40f;
    [SerializeField] Transform modelTransform;
    //TODO. 원래는 외부에서 속도를 받아야하긴하는데. 그냥 고정 속도로 해보자. 그게 나을듯
    
    float maxBulletSpeed = 5.0f;
    float moveDelay = 0.2f; //수리검이 적과 닿고있을때는 속도가 20%로 감소함
    float curBulletSpeed;
    bool canMove = false;

    //적을 파악하는 딕셔너리
    [SerializeField] float lifeTime = 10f;
    [SerializeField] float damageInterval = 0.1f;
    Dictionary<Enemy, float> takeDamaeEnemyDict = new Dictionary<Enemy, float>();

    private void OnDestroy()
    {
        UnSubscribeAll();
    }

    private void OnTriggerStay(Collider other)
    {
        //other이 Enemy레이어가 아니라면 return;
        if (((1 << other.gameObject.layer) & TargetManager.Instance.EnemyLayer.value) == 0) return;

        if(other.TryGetComponent(out Enemy enemy))
        {
            //처음 부딫힌다면 딕셔너리에 넣기
            if(!takeDamaeEnemyDict.ContainsKey(enemy))
            {
                enemy.OnEnemyDead += EnemyDead;
                takeDamaeEnemyDict.Add(enemy, float.MinValue);
            }
            //n초마다 데미지를 준다.
            if(takeDamaeEnemyDict.ContainsKey(enemy))
            {
                float time = takeDamaeEnemyDict[enemy];
                if(time <= Time.time)
                {
                    takeDamaeEnemyDict[enemy] = Time.time + damageInterval;
                    DamageInfo damageInfo = MakeDamageInfo(enemy);
                    int calcDamage = DamageManager.Instance.CalcDamage(damageInfo);
                    enemy.TakeDamage(calcDamage);
                }
            }
        }

        if(curBulletSpeed == maxBulletSpeed && takeDamaeEnemyDict.Count > 0)
        {
            curBulletSpeed = maxBulletSpeed * moveDelay;
        }

    }

    

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            if(takeDamaeEnemyDict.ContainsKey(enemy))
            {
                enemy.OnEnemyDead -= EnemyDead;
                takeDamaeEnemyDict.Remove(enemy);
            }
        }
        if(takeDamaeEnemyDict.Count == 0)
        {
            curBulletSpeed = maxBulletSpeed;
        }
    }
    private void EnemyDead(Enemy enemy)
    {
        if (takeDamaeEnemyDict.ContainsKey(enemy))
        {
            takeDamaeEnemyDict.Remove(enemy);
        }
        else
        {
            MyLog.LogError("몬스터가 여기 무조건 있어야하는데 이상함");
        }

        if (takeDamaeEnemyDict.Count == 0) 
        {
            curBulletSpeed = maxBulletSpeed;
        }
    }

    public override void Shoot()
    {
        curBulletSpeed = maxBulletSpeed;
        canMove = true;
        //StartCoroutine(RotateObject());
        Invoke(nameof(DestroySelf), lifeTime);
    }
    private void UnSubscribeAll()
    {
        foreach (var item in takeDamaeEnemyDict.Keys)
        {
            if(item != null)
            {
                item.OnEnemyDead -= EnemyDead;
            }
        }
        
    }
    protected override void DestroySelf()
    {
        UnSubscribeAll();
        takeDamaeEnemyDict.Clear();
        Destroy(this.gameObject);
    }

    protected override void Init()
    {

    }

    protected override void Move()
    {
        MyLog.Log($"현재속도: " +curBulletSpeed); 
        rb.MovePosition(rb.position + transform.forward * curBulletSpeed * Time.fixedDeltaTime);
    }
    private void Update()
    {
        modelTransform.Rotate(0, rotateSpeed *Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {
        //물리적으로 앞으로 이동한다.
        if(canMove) 
        { 
            Move(); 
        }
    }
}
