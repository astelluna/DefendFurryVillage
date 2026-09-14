using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //오브젝트풀 프리팹,사이즈 , 선언
    //[SerializeField] Enemy _enemyPrefab;
    [SerializeField] int poolSize = 10;
    ObjPool<Enemy> _enemyPool;
    public event Action<int> _getExpPoint;


    public static EnemySpawner Instance { get; private set; }

    //주의 : 원본 프리팹의 Enemy만 검색하면서 쓰고 풀에서 꺼낸 Enemy는 사용하면 안됨
    Dictionary<Enemy, ObjPool<Enemy>> enemyDict = new Dictionary<Enemy, ObjPool<Enemy>>();

    public event Action OnSpawnEnd;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    //public void MakeEnemy(Enemy enemyPrefab)
    public void MakeEnemy(List< Enemy> enemyPrefabList)
    {
        //해당 enemy를 n개 미리 생성해둔다.
        foreach(Enemy enemyPrefab in enemyPrefabList )
        {
            if (enemyDict.ContainsKey(enemyPrefab))
            {
                return;
            }
            ObjPool<Enemy> enemyPool = new ObjPool<Enemy>(enemyPrefab, poolSize, gameObject.transform);
            enemyDict.Add(enemyPrefab, enemyPool);
        }

        OnSpawnEnd?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    //생성 코드
    public Enemy GetPrefabByPool(Transform trf, Enemy enemy)
    {
        if (!enemyDict.ContainsKey(enemy)) { return null; }

        ObjPool<Enemy> enemyPool = enemyDict[enemy];
        Enemy enemyBuf = enemyPool.GetObject();
        enemyBuf.transform.position = trf.position;
        enemyBuf.transform.rotation = trf.rotation;
        enemyBuf.SetObjectPool(enemyPool);
        //enemyBuf.OnEnemyDead += ReturnPrefabToPool;

        return enemyBuf;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    private void ReturnPrefabToPool(Enemy enemy)
    {
        //현재는 enemy에서 직접 반환해서 안씀
        //추후에 필요할때 넣자
        enemy.OnEnemyDead -= ReturnPrefabToPool;
        enemy.ReturnToPool();

        //if (!enemyDict.ContainsKey(enemy)) { return null; } //주의. 이렇게 딕셔너리로 검색하면 안된다 -> enemy는 원본 프리팹이 아니라 꺼낸 프리팹임

        //경험치를 주웠을때 
        // _getExpPoint?.Invoke(10);
    }
}
