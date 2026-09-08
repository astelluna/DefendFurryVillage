using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct EnemyRuntimeStat
{
    public int attack { get; private set; }
    public int defense { get; private set; }
    //public int hp { get; private set; }
    public float attackRate { get; private set; }
    public float moveSpeed { get; private set; }
    public float penetration { get; private set; }
    //public int experience { get; private set; }


    public void SetEnemyData(int atk, int def, float atkRate, float moveSpd, float penet)
    {
        attack = atk;
        defense = def;
        attackRate = atkRate;
        moveSpeed = moveSpd;
        penetration = penet;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    
    //[SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    //상태이상 처리
    [SerializeField] EnemyStatusEffectController statusEffectController;

    bool isMove = true;
    //공격을 추가한다면?
    //int damage;
    //int attack = 10;
    //float attackRate = 1f;
    float curAttackRate;
    WaitForSeconds delay;
    Coroutine attackCor;

    private Coroutine exitGraceCor;


    Coroutine magnetCoroutine;
    bool isMagnet = false;
    Vector3 magnetPos;
    float magnetSpeed = 50;

    //몬스터 능력치들을 SO로 등록할수있게
    [SerializeField] EnemyDataSO enemyDataSO;
    //int experience;
    int curHp;
    EnemyRuntimeStat runtimeStat;

    private void Init()
    {
        curHp = enemyDataSO.maxHp;
        UpdateEnemyStat();
    }

    public void UpdateEnemyStat()
    {
        runtimeStat = new EnemyRuntimeStat();
        //기본 데이터와. 런타임 데이터를 합해서 업데이트.
        //베이직으로 기본 셋팅
        int atk = enemyDataSO.attack;
        int def = enemyDataSO.defense;
        float atkRate = enemyDataSO.attackRate;
        float moveSpeed = enemyDataSO.moveSpeed;
        float penetration = enemyDataSO.penetration;
        //버프나 디버프는 아직없음. 추후 있으면 추가. 일단 이렇게만 만들어두자.

        //설정
        runtimeStat.SetEnemyData(atk, def, atkRate, moveSpeed, penetration);
        if(curAttackRate != runtimeStat.attackRate)
        {
            curAttackRate = runtimeStat.attackRate;
            delay = new WaitForSeconds(curAttackRate);
        }
        
    }

    private void OnEnable()
    {
        isMove = true;
        attackCor = null;
        exitGraceCor = null;

        Init();
    }
    private void FixedUpdate()
    {
        if(isMove && !isMagnet)
        {
            if(runtimeStat.moveSpeed > 0)
            {
                rb.MovePosition(transform.position + transform.forward * -runtimeStat.moveSpeed * Time.fixedDeltaTime);
            }
        }
        
    }
    private void OnDisable()
    {
        if (attackCor != null)
        {
            StopCoroutine(attackCor);
            attackCor = null;
        }
        if (exitGraceCor != null)
        {
            StopCoroutine(exitGraceCor);
            exitGraceCor = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //성벽이 아니었다면 return;
        if ((TargetManager.Instance.CastleWallLayer.value & (1 << other.gameObject.layer)) == 0) return;
        // 다시 붙었으니 exit 유예 취소
        if (exitGraceCor != null)
        {
            StopCoroutine(exitGraceCor);
            exitGraceCor = null;
        }
        //성벽 트리거가 감지되었다면?
        //TODO. 이제 성벽을 공격해야함.
        if (other.TryGetComponent(out CastleWall castleWall))
        {
            if(attackCor == null)
            {
                attackCor = StartCoroutine(AttackRoutine(castleWall));
            }
        }
        else
        {
            MyLog.LogError("성벽 레이어인데 스크립트가 없습니다.");
        }
        isMove = false;

    }

    private void OnTriggerExit(Collider other)
    {
        if ((TargetManager.Instance.CastleWallLayer.value & (1 << other.gameObject.layer)) == 0) return;

        isMove = true;

        if (exitGraceCor == null)
            exitGraceCor = StartCoroutine(ExitGraceRoutine());
    }
    
    IEnumerator AttackRoutine(CastleWall castleWall)
    {
        while(true)
        {
            if (castleWall == null) break;
            castleWall.TakeDamage(runtimeStat.attack);
            yield return delay;
        }
    }

    private IEnumerator ExitGraceRoutine()
    {
        yield return new WaitForSeconds(0.2f); // 넉백 회복 정도의 여유시간

        if (attackCor != null)
        {
            StopCoroutine(attackCor);
            attackCor = null;
        }
        exitGraceCor = null;
    }

    public void TakeDamage(int dmg)
    {
        //TODO -> 데미지 처리
        curHp -= dmg;
        MyLog.Log($"{dmg} 만큼 데미지 받음, HP: {curHp}");
        
        if(curHp <= 0 )
        {
            Dead();
        }
    }

    private void Dead()
    {
        MyLog.Log("사망처리");
        LevelUpManager.Instance.GetExperience(enemyDataSO.experience);
        Destroy(this.gameObject);
    }

    public void OnHitDelay(int dmg, float delayTime)
    {
        //일정시간 이후에 데미지를 받아야 할때 사용한다.
        StartCoroutine(WaitForDelay(dmg,delayTime));
    }

    IEnumerator WaitForDelay(int dmg, float delayTime)
    {
        //TODO. 기다리는동안 롤 처형시간 이펙트처럼 몸에 이펙트 나와야하면 표시하면 될듯
        yield return new WaitForSeconds(delayTime);
        TakeDamage(dmg);
    }

    public void StartEnemyMagnet(Vector3 centerPos, float pullDistance)
    {
        Vector3 direction = (centerPos - rb.position).normalized;
        float distance = Vector3.Distance(rb.position, centerPos);

        float moveDistance = Mathf.Min(pullDistance, distance);

        magnetPos = rb.position + direction * moveDistance;
        if (magnetCoroutine != null) 
        { 
            StopCoroutine(magnetCoroutine);
            magnetCoroutine = null;
        }
        isMagnet = true;
        magnetCoroutine = StartCoroutine(MagnetMove());
    }

    IEnumerator MagnetMove()
    {
        while(true)
        {
            float dist = Vector3.Distance(rb.position, magnetPos);
            if(dist < 0.05f)
            {
                isMagnet = false;
                magnetCoroutine = null;
                yield break;
            }
            Vector3 nextPos = Vector3.MoveTowards(rb.position, magnetPos, Time.fixedDeltaTime * magnetSpeed);
            rb.MovePosition(nextPos);
            yield return new WaitForFixedUpdate();
        }
        
    }

    public void KnockBack(float distance)
    {

        Vector3 pos = rb.position;
        pos.z += distance;

        rb.MovePosition(pos);
    }

    public void AddStatusEffect(IStatusEffect effect)
    {
        statusEffectController.AddStatusEffect(effect);
    }

    public void RegisterEffect(GameObject vfx)
    {
        //TODO. 추후에 이펙트 추가하자
    }
    public void RemoveEffect(GameObject vfx)
    {
        //TODO. 추후에 이펙트 제거 하는거 넣자.
    }
}

