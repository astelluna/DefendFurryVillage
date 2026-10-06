using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "StageWave/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    //몇개의 웨이브가 존재하는지, 스테이지 번호
    [field: SerializeField] public int StageId { get; private set; }
    [field: SerializeField] public string StageName { get; private set; }
    [field: SerializeField] public string StageDesc { get; private set; }
    [field: SerializeField] public List<WaveDataSO> WaveDataList { get; private set; } = new List<WaveDataSO>();
}
