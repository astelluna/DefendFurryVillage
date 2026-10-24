using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //어떤 스테이지를 선택하면 해당 스테이지의 정보대로 적을 소환한다. -> 일단 직렬화로 테스트 

    [Header("!테스트!. 테스트가 필요하면 직접 넣기")]
    [SerializeField] StageDataSO stageDataSO; //테스트할떄는 수동으로 넣어지고, 정상적일땐 알아서 데이터 설정됨
    

    [SerializeField] float spawnTime = 0.1f;

    
    WaitForSeconds delay;
    int curWaveIndex = 0;
    float nextWaveStartTime = 1f;
    //bool isSpawning = false;

    //현재 웨이브에서 살아있는 적 갯수
    int remainSpawnCount = 0;
    List<Enemy> aliveEnemyList = new List<Enemy>();

    public event System.Action<StageDataSO> StageClearRequest;

    #region 인게임씬에서 외부에서 전달받아서 정보 SET 하는 필드들
    StagePanel stagePanel; //스테이지 정보 표시하는 UI
    StageEnemySpawnPosition stageEnemySpawnPosition; //소환 위치 담아둔 게임오브젝트
    #endregion

    protected override void OnAwake()
    {
    }
    private void Start()
    {
        delay = new WaitForSeconds(spawnTime);
        curWaveIndex = 0;
        
    }
    private void OnDestroy()
    {
        CancelInvoke();
        foreach (var item in aliveEnemyList)
        {
            if (item != null) { item.OnEnemyDead -= EnemyDead; }
        }
        aliveEnemyList.Clear();
    }

    public async Task InitForIngameScene()
    {
        //인게임 씬에 진입했을때 해야하는 행동 정의
        await RegisterEnemySpawner();
    }
    private async Task RegisterEnemySpawner()
    {
        HashSet<EnemyDataSO> enemyHash = new HashSet<EnemyDataSO>();
        foreach (var item in stageDataSO.WaveDataList)
        {
            foreach (var enemy in item.EnemieList)
            {
                if (!enemyHash.Contains(enemy))
                {
                    enemyHash.Add(enemy);
                }
            }
        }
        //모든 종류를 파악한다음에 EnemySpawner에 위임.
        EnemySpawner.Instance.OnSpawnEnd += StageInitComplete;

        try
        {
            await EnemySpawner.Instance.MakeEnemy(enemyHash.ToList());
        }
        catch(System.Exception e)
        {
            MyLog.LogError($"스테이지 에러 {e}");
        }
    }

    private void StageInitComplete()
    {
        EnemySpawner.Instance.OnSpawnEnd -= StageInitComplete;
    }
    public void WaveStart()
    {
        //Invoke(nameof(StartWaveDelay_Test),3f);

        curWaveIndex = 0;
        StartWaveDelay_Test();
        //StartWave(curWaveIndex);
    }
    public void SetStageData(StageDataSO data)
    {
        stageDataSO = data;
    }
    public void SetIngameData(StagePanel panel, StageEnemySpawnPosition  spawnPosition)
    {
        //인게임에서 필요한 스테이지 관련 정보를 넣음
        stagePanel = panel;
        stageEnemySpawnPosition = spawnPosition;
    }
    private void StartWaveDelay_Test()
    {
        StartWave(curWaveIndex);
    }


    private void StartWave(int index)
    {
        MyLog.Log($"{index}번째 웨이브 시작");
        if (index < 0 || index >= stageDataSO.WaveDataList.Count)
        {
            MyLog.LogError("웨이브 인덱스가 이상함");
            return;
        }
        var curWaveData = stageDataSO.WaveDataList[index];
        StartCoroutine(spawnCor(curWaveData,index));
    }

    IEnumerator spawnCor(WaveDataSO waveData,int waveIndex)
    {
        remainSpawnCount = waveData.EnemieList.Count;
        int cnt = 0;
        //UI 갱신
        RequestUpdateStagePanel();
        //적 소환 시작
        while (cnt < waveData.EnemieList.Count)
        {
            //오브젝트풀에서 가져오도록 하자.

            //Enemy enemyData = waveData.EnemieList[cnt];
            EnemyDataSO enemyData = waveData.EnemieList[cnt];

            Enemy enemyObj = EnemySpawner.Instance.GetPrefabByPool(stageEnemySpawnPosition.transform, enemyData);
            //Enemy enemyObj =  Instantiate(enemyData, transform);
            float value = Random.Range(-3f, 3f);
            Vector3 pos = enemyObj.transform.position;
            pos.x = value;
            enemyObj.transform.position = pos;
            enemyObj.OnEnemyDead += EnemyDead;
            enemyObj.gameObject.SetActive(true);
            aliveEnemyList.Add(enemyObj);
            cnt++;
            yield return delay;
        }
    }

    private void EnemyDead(Enemy enemy)
    {
        if(aliveEnemyList.Contains(enemy))
        {
            remainSpawnCount--;
            enemy.OnEnemyDead -= EnemyDead;
            aliveEnemyList.Remove(enemy);
        }
        else
        {
            MyLog.LogError("뭔가 이상. 여기에 무조건 있어야함");
        }
        //UI 갱신
        RequestUpdateStagePanel();

        //몬스터가 전부 죽었다면, 다음 스테이지가 있는지 확인하고 스테이지 시작해줌
        if (CheckWaveClear())
        {
            Invoke(nameof(StartNextWaveDelyed), nextWaveStartTime);
        }
        //스테이지가 전부 끝났으면 종료 이벤트 발동(결과창 띄울거임)
        if(CheckStageClear())
        {
            MyLog.Log("스테이지 종료 - 이벤트 발동");
            StageClearRequest?.Invoke(stageDataSO);
        }
        
    }
    private void RequestUpdateStagePanel()
    {
        stagePanel.UpdateStageInfo(curWaveIndex + 1, stageDataSO.WaveDataList.Count, aliveEnemyList.Count);
    }

    private void StartNextWaveDelyed()
    {
        curWaveIndex += 1;
        StartWave(curWaveIndex);

    }

    private bool CheckWaveClear()
    {
        if (remainSpawnCount != 0) return false;
        if (curWaveIndex + 1 >= stageDataSO.WaveDataList.Count) return false;

        return true;
    }
    private bool CheckStageClear()
    {
        if(remainSpawnCount == 0 && curWaveIndex + 1 >= stageDataSO.WaveDataList.Count)
        {
            return true;
        }
        return false;
    }


}
