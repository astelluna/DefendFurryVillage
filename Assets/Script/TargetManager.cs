using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [field:SerializeField] public LayerMask EnemyLayer { get; private set; }
    [field:SerializeField] public LayerMask CastleWallLayer { get; private set; }
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }

    //플레이어가 총알을 쏠때 어떤 타겟 방향으로 총알을 쏴야하는지 결정한다.
    [SerializeField] float radius;
    //[SerializeField] LayerMask enemyLayer;
    Collider[] result = new Collider[30];
    //Transform enemyTarget;
    List<(float, Transform)> sortedEnemyList = new List<(float, Transform)>(); //적의 거리, transform정보가 오름차순 저장된 리스트
    //float nearestDistance;
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

    private void FindEnemyTarget()
    {
        //원형으로 체크를 해볼까? Enemy 타겟을 찾는거임
        var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, radius, result, EnemyLayer);
        sortedEnemyList.Clear();
        if (enemyCount == 0)
        {
            //enemyTarget = null;
            return;
        }
        //Transform enemyTransform = null;
        //nearestDistance = float.MaxValue;
        //List<(float, Transform)> transformList = new List<(float, Transform)>();
        
        for (int i = 0; i < enemyCount; i++)
        {
            //여러 위치를 측정해서. 저장해둬보자. 최대 5개? 이렇게 해볼까
            //거리 측정
            float distance = Vector3.Distance(transform.position, result[i].transform.position);
            //if (distance < nearestDistance)
            //{
            //    nearestDistance = distance;
            //    enemyTransform = result[i].transform;
            //}

            sortedEnemyList.Add((distance, result[i].transform));

        }
        //enemyTarget = enemyTransform;

        sortedEnemyList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        //return enemyTransform;
    }

    //코루틴으로 0.1초마다 실행하자
    IEnumerator FindTargetCor()
    {
        while(true)
        {
            FindEnemyTarget();
            //MyLog.Log(enemyTarget);
            yield return delay;
        }
    }
    public List<Transform> GetEnemyTarget(float range, int maxCount)
    {
        //maxCount개만큼 최소순위부터 계속 반환을 해본다.
        List<Transform> enemyTargetList = new List<Transform>();
        if (sortedEnemyList == null)
        {
            MyLog.LogError("에러떠서 기본꺼 넣어서 보냅니다");
            return enemyTargetList;
        }
        for (int i=0; i< maxCount; i++)
        {
            if (sortedEnemyList.Count <= i) break;
            var k = sortedEnemyList;
            if (k[i].Item2 == null) continue;
            if (k[i].Item1 > range) break;
            
            
            enemyTargetList.Add(sortedEnemyList[i].Item2);
        }

        return enemyTargetList;
    }

}
