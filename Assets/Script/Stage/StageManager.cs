using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StageProgressData
{
    //현재 최대로 클리어한 월드, 스테이지 진행 정보에 대해서 기록. (초기에 DB에서 읽어서 설정됨)
    //다음에 플레이할수 있는 스테이지를 지정한다.
    public long worldId { get; private set; }
    public long stageId { get; private set; }
    public void SetData(long wld, long stg)
    {
        worldId = wld;
        stageId = stg;
    }

    public void SetData(WorldStageInfo info)
    {
        //설정 명령이 들어오면, 스테이지 마지막인지 확인해서 월드 count를 +1 해줌
        int worldIndex = info.WorldIndex;
        int stageIndex = 1; //초기 1부터 시작
        if(info.StageIndex == info.maxStageIndex)
        {
            worldIndex += 1;
        }
        else
        {
            stageIndex = info.StageIndex + 1;
        }
        worldId = worldIndex;
        stageId = stageIndex;
    }
}
public class WorldStageInfo
{
    //현재 선택한 스테이지 정보를 저장하기 위해서 새로 선언함
    public int WorldIndex { get; private set; }
    public int StageIndex { get; private set; }
    public int maxStageIndex { get; private set; } //현재 스테이지의 최대 갯수 지정
    public Material worldMaterial { get; private set; }
    public void SetData(int worldIdx, int stageIdx,int maxStageIdx, Material material)
    {
        WorldIndex = worldIdx;
        StageIndex = stageIdx;
        maxStageIndex = maxStageIdx;
        worldMaterial = material;
    }
}
public class StageManager : Singleton<StageManager>
{
    //어떤 스테이지를 선택하면 해당 스테이지의 정보대로 적을 소환한다. -> 일단 직렬화로 테스트 
    [Header("월드 갯수만큼 데이터 지정")]
    [SerializeField] List<AssetReferenceT<WorldDataSO>> allWorldDataRefs;
    public IReadOnlyList<WorldDataSO> AllWorldDataSoList => allWorldDataSOList;
    StageDataSO selectedStageData;
    WorldStageInfo worldStageInfo = new WorldStageInfo(); //현재 선택한 월드,스테이지 정보
    public StageProgressData BestStageProgressData { get; private set; } = new StageProgressData(); //DB에서 정보를 받아서 셋팅되는 최고기록 스테이지 진행상황
    [Header("인게임- 몬스터 소환 주기")]
    [SerializeField] float spawnTime = 0.1f;

    
    WaitForSeconds delay;
    int curWaveIndex = 0;
    float nextWaveStartTime = 1f;
    //bool isSpawning = false;

    //현재 웨이브에서 살아있는 적 갯수
    int remainSpawnCount = 0;
    List<Enemy> aliveEnemyList = new List<Enemy>();

    public event System.Action<StageDataSO, WorldStageInfo> StageClearRequest;

    #region 인게임씬에서 외부에서 전달받아서 정보 SET 하는 필드들
    StagePanelInGame stageInGamePanel; //스테이지 정보 표시하는 UI
    StageEnemySpawnPosition stageEnemySpawnPosition; //소환 위치 담아둔 게임오브젝트
    #endregion

    // 월드 
    //외부에서 초기화 작업 끝났는지 확인하는 Task
    private readonly TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
    Dictionary<string, WorldDataSO> allWorldSODict = new Dictionary<string, WorldDataSO>(); //검색용 딕셔너리
    List<WorldDataSO> allWorldDataSOList = new List<WorldDataSO>();

    private async Task Init()
    {
        try
        {
            await LoadWorldDataSO_Init();
            initTcs.TrySetResult(true);
            MyLog.Log($"월드 정보 설정 완료");
        }
        catch (Exception e)
        {
            MyLog.LogError($"월드 초기화 실패:{e}");
            initTcs.TrySetException(e);
        }
    }
    private async Task LoadWorldDataSO_Init()
    {
        //어드레서블로 되어있는 캐릭터 데이터들을 계속 불러와본다 
        foreach (var item in allWorldDataRefs)
        {
            WorldDataSO worldDataSO = await AddressableManager.Instance.GetAssetAsync_Asset<WorldDataSO>(item);
            if (allWorldSODict.ContainsKey(worldDataSO.worldName))
            {
                MyLog.LogError("SO에 월드ID가 중복입니다");
                continue;
            }
            //초기화
            allWorldSODict.Add(worldDataSO.worldName, worldDataSO);
            allWorldDataSOList.Add(worldDataSO);
        }
    }


    

    protected override void OnAwake()
    {
    }
    private async void Start()
    {
        
        delay = new WaitForSeconds(spawnTime);
        curWaveIndex = 0;
        await Init();
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

    #region 인게임씬에서 사용되는 메서드
    public async Task InitForIngameScene()
    {
        //인게임 씬에 진입했을때 해야하는 행동 정의
        await RegisterEnemySpawner();
    }
    private async Task RegisterEnemySpawner()
    {
        HashSet<EnemyDataSO> enemyHash = new HashSet<EnemyDataSO>();
        foreach (var item in selectedStageData.WaveDataList)
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
    public void SetStageData(StageDataSO data, WorldStageInfo worldStageData)
    {
        selectedStageData = data;
        worldStageInfo = worldStageData;
    }
    public Material GetFloorData()
    {
        return worldStageInfo.worldMaterial;
    }
    public void SetIngameData(StagePanelInGame panel, StageEnemySpawnPosition  spawnPosition)
    {
        //인게임에서 필요한 스테이지 관련 정보를 넣음
        stageInGamePanel = panel;
        stageEnemySpawnPosition = spawnPosition;
    }
    private void StartWaveDelay_Test()
    {
        StartWave(curWaveIndex);
    }


    private void StartWave(int index)
    {
        MyLog.Log($"{index}번째 웨이브 시작");
        if (index < 0 || index >= selectedStageData.WaveDataList.Count)
        {
            MyLog.LogError("웨이브 인덱스가 이상함");
            return;
        }
        var curWaveData = selectedStageData.WaveDataList[index];
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
            float value = UnityEngine.Random.Range(-3f, 3f);
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
            StageClearRequest?.Invoke(selectedStageData, worldStageInfo);
        }
        
    }
    private void RequestUpdateStagePanel()
    {
        stageInGamePanel.UpdateStageInfo(curWaveIndex + 1, selectedStageData.WaveDataList.Count, aliveEnemyList.Count);
    }

    private void StartNextWaveDelyed()
    {
        curWaveIndex += 1;
        StartWave(curWaveIndex);

    }

    private bool CheckWaveClear()
    {
        if (remainSpawnCount != 0) return false;
        if (curWaveIndex + 1 >= selectedStageData.WaveDataList.Count) return false;

        return true;
    }
    private bool CheckStageClear()
    {
        if(remainSpawnCount == 0 && curWaveIndex + 1 >= selectedStageData.WaveDataList.Count)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 일반 스테이지 데이터
    public void SetProgressDataFromDB(ProgressDataDB progressDataDB)
    {
        BestStageProgressData.SetData(progressDataDB.WorldId, progressDataDB.StageId);
    }
    public async Task  CheckAndUpdateStageBestRecord(WorldStageInfo worldStageInfo)
    {
        //기록하기전에 클리어했으면 다음 
        //BestStageProgressData 형식은 다음에 플레이할수있는 스테이지를 표시하므로 worldStageInfo로 BestStageProgressData형식을 만든다
        StageProgressData stageProgressData = new StageProgressData();
        stageProgressData.SetData(worldStageInfo);


        //현재 스테이지 클리어 기록이 기록 경신을 한것인지 체크하고, 기록 경신했으면 업데이트
        if (stageProgressData.worldId > BestStageProgressData.worldId || 
            (stageProgressData.worldId >= BestStageProgressData.worldId && stageProgressData.stageId > BestStageProgressData.stageId))
        {
            //기록 경신
            await FirestoreManager.Instance.ProgressSave(stageProgressData.worldId, stageProgressData.stageId);
        }
    }
    #endregion

}
