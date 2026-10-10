using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawner : MonoBehaviour
{
    //오브젝트풀 프리팹,사이즈 , 선언
    public static EnemySpawner Instance { get; private set; }
    [SerializeField] int poolSize = 10;   

    //주의 : 원본 프리팹의 Enemy만 검색하면서 쓰고 풀에서 꺼낸 Enemy는 사용하면 안됨
    Dictionary<Enemy, ObjPool<Enemy>> enemyDict = new Dictionary<Enemy, ObjPool<Enemy>>();

    Dictionary<EnemyDataSO, Enemy> enemyPrefabDict = new Dictionary<EnemyDataSO, Enemy>(); //미리 저장


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
    public async Task MakeEnemy(List< EnemyDataSO> enemyDataList)
    {
        //해당 enemy를 n개 미리 생성해둔다.
        Task<Enemy>[] taskArray = new Task<Enemy>[enemyDataList.Count];

        //한번 비동기로 전체를 불러온다
        for(int i=0; i< enemyDataList.Count; i++)
        {
            //taskArray[i] = CharacterManager.Instance.AddressableLoadFunc<Enemy>(enemyDataList[i].EnemyPrefab);
            taskArray[i] = AddressableManager.Instance.GetAssetAsync_Asset<Enemy>(enemyDataList[i].EnemyPrefab);
        }
        Enemy[] enemyArray =  await Task.WhenAll(taskArray); //비동기 대기 이후 
        //해당 enemy를 n개 미리 생성해둔다.
        for (int i=0; i< enemyArray.Length; i++)
        {
            if (enemyPrefabDict.ContainsKey(enemyDataList[i]))
            {
                continue;
            }
            //오브젝트풀 생성하고 반환을 위한 딕셔너리, 나중에 생성을 위한 딕셔너리를 등록함
            ObjPool<Enemy> enemyPool = new ObjPool<Enemy>(enemyArray[i], poolSize, gameObject.transform);
            //enemyDict.Add(enemyArray[i], enemyPool);
            enemyDict[enemyArray[i]] = enemyPool;
            enemyPrefabDict.Add(enemyDataList[i], enemyArray[i]);
        }
        OnSpawnEnd?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        //TODO. 어드레서블 전부 해제 필요
    }
    //생성 코드
    public Enemy GetPrefabByPool(Transform trf, EnemyDataSO enemyData)
    {
        if (!enemyPrefabDict.ContainsKey(enemyData)) 
        {
            MyLog.LogError("적 프리팹이 생성이 안되어있습니다. 뭔가 에러");
            return null; 
        }
        //오브젝트풀을 알아내서 enemy를 하나 가져옴
        Enemy enemy = enemyPrefabDict[enemyData];
        ObjPool<Enemy> enemyPool = enemyDict[enemy];
        Enemy enemyBuf = enemyPool.GetObject();
        //정보 설정
        enemyBuf.SetEnemyDataSO(enemyData);
        enemyBuf.transform.position = trf.position;
        enemyBuf.transform.rotation = trf.rotation;
        enemyBuf.SetObjectPool(enemyPool);
        enemyBuf.Init();
        return enemyBuf;
    }


    public Enemy GetPrefabByPool_TEST(Transform trf, Enemy enemy)
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
