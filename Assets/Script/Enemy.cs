using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    int damage;
    [SerializeField] float speed;
    //추가 빙결, 그런 효과들은 따로 처리
    //List<IBulletEffect> effectList = new List<IBulletEffect>();

    [SerializeField] Rigidbody rb;

    bool isMove = true;
    //공격을 추가한다면?
    int attack = 10;
    float attackRate = 1f;
    WaitForSeconds delay;
    Coroutine attackCor;

    private Coroutine exitGraceCor;
    //public void SetEnemyInfo(int dmg, float spd, List<IBulletEffect> effects)
    //{
    //    damage = dmg;
    //    speed = spd;
    //    effectList = effects;
    //}
    private void Awake()
    {
        delay = new WaitForSeconds(attackRate);
    }

    private void OnEnable()
    {
        isMove = true;
        attackCor = null;
        exitGraceCor = null;
    }
    private void FixedUpdate()
    {
        if(isMove)
        {
            rb.MovePosition(transform.position + transform.forward * -speed * Time.fixedDeltaTime);
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
            Debug.LogError("성벽 레이어인데 스크립트가 없습니다.");
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
            castleWall.TakeDamage(attack);
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

    void OnHit()
    {
        //TODO -> 충돌 처리
    }
}

