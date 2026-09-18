using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //어떤 스테이지를 선택하면 해당 스테이지의 정보대로 적을 소환한다. -> 일단 직렬화로 테스트 
    [SerializeField] StageDataSO testStageData;

    [SerializeField] float spawnTime = 0.1f;

    //스테이지 정보 표시하는 UI
    [SerializeField] StagePanel stagePanel;

    WaitForSeconds delay;
    int curWaveIndex = 0;
    float nextWaveStartTime = 1f;
    //bool isSpawning = false;

    //현재 웨이브에서 살아있는 적 갯수
    int remainSpawnCount = 0;
    List<Enemy> aliveEnemyList = new List<Enemy>();

    private void Start()
    {
        delay = new WaitForSeconds(spawnTime);
        curWaveIndex = 0;
        RegisterEnemySpawner();
    }
    private void RegisterEnemySpawner()
    {
        //Enemy 오브젝트풀에 현재 스테이지 정보로 적을 생성하라고 지시. 이벤트를 통해서 다 설정되면 웨이브가 시작됨
        HashSet<Enemy> enemyHash = new HashSet<Enemy>();
        foreach(var item in testStageData.WaveDataList)
        {
            foreach(var enemy in item.EnemieList)
            {
                if(!enemyHash.Contains(enemy))
                {
                    enemyHash.Add(enemy);
                }
            }
        }
        //모든 종류를 파악한다음에 EnemySpawner에 위임.
        EnemySpawner.Instance.OnSpawnEnd += WaveStart;
        EnemySpawner.Instance.MakeEnemy(enemyHash.ToList());
    }
    private void WaveStart()
    {
        EnemySpawner.Instance.OnSpawnEnd -= WaveStart;
        StartWave(curWaveIndex);
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
    private void StartWave(int index)
    {
        MyLog.Log($"{index}번째 웨이브 시작");
        if (index < 0 || index >= testStageData.WaveDataList.Count)
        {
            MyLog.LogError("웨이브 인덱스가 이상함");
            return;
        }
        var curWaveData = testStageData.WaveDataList[index];
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



            Enemy enemyData = waveData.EnemieList[cnt];
            Enemy enemyObj = EnemySpawner.Instance.GetPrefabByPool(transform, enemyData);
            //Enemy enemyObj =  Instantiate(enemyData, transform);
            float value = Random.Range(0f, 8f);
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
        
    }
    private void RequestUpdateStagePanel()
    {
        stagePanel.UpdateStageInfo(curWaveIndex + 1, testStageData.WaveDataList.Count, aliveEnemyList.Count);
    }

    private void StartNextWaveDelyed()
    {
        curWaveIndex += 1;
        StartWave(curWaveIndex);

    }

    private bool CheckWaveClear()
    {
        if (remainSpawnCount != 0) return false;
        if (curWaveIndex + 1 >= testStageData.WaveDataList.Count) return false;

        return true;
    }

}
