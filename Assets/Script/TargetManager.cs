using System.Collections;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [field:SerializeField] public LayerMask EnemyLayer { get; private set; }

    //플레이어가 총알을 쏠때 어떤 타겟 방향으로 총알을 쏴야하는지 결정한다.
    [SerializeField] float radius;
    //[SerializeField] LayerMask enemyLayer;
    Collider[] result = new Collider[30];
    Transform enemyTarget;
    float nearestDistance;
    //타겟 찾는거는 제일 가까운거 찾아두고, 무기 사거리에 따라서 알아서 공격하도록 하자
    float delayTime = 0.1f;
    WaitForSeconds delay;
    Coroutine findEnemyCor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        delay = new WaitForSeconds(delayTime);
    }
    private void Start()
    {
        if(findEnemyCor == null)
        {
            findEnemyCor = StartCoroutine(FindTargetCor());
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
        if(findEnemyCor != null)
        {
            StopCoroutine(findEnemyCor);
            findEnemyCor = null;
        }
    }

    private Transform FindEnemyTarget()
    {
        //원형으로 체크를 해볼까? Enemy 타겟을 찾는거임
        var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, radius, result, EnemyLayer);
        if (enemyCount == 0)
        {
            enemyTarget = null;
            return null;
        }
        Transform enemyTransform = null;
        nearestDistance = float.MaxValue;
        for (int i = 0; i < enemyCount; i++)
        {
            //내 앞쪽에 있는지 확인? 어짜피 벽으로 가려있는데 안해도되지않을까


            //거리 측정
            float distance = Vector3.Distance(transform.position, result[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                enemyTransform = result[i].transform;
            }
        }
        enemyTarget = enemyTransform;
        return enemyTransform;
    }

    //코루틴으로 0.1초마다 실행하자
    IEnumerator FindTargetCor()
    {
        while(true)
        {
            FindEnemyTarget();
            Debug.Log(enemyTarget);
            yield return delay;
        }
    }
    public Transform GetEnemyTarget(float range)
    {
        //무기들이 사거리를 넣으면, 현재 사거리에 포함되는지 확인해서 반환
        if(nearestDistance <= range)
        {
            return enemyTarget;
        }
        else
        {
            return null;
        }
    }
}
