using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StageWave/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    //몇개의 웨이브가 존재하는지, 스테이지 번호
    [field: SerializeField] public int StageId { get; private set; } //스테이지 형식 예시) 앞1자리는 스테이지id고유번호, 뒤 두자리는 월드번호, 뒤2자리는 스테이지 번호 30103 : 1월드의 3번 스테이지
    [field: SerializeField] public string StageName { get; private set; }
    [field: SerializeField] public string StageDesc { get; private set; }
    [field: SerializeField] public RewardDataSO RewardData { get; private set; } //보상 정보
    [field: SerializeField] public List<WaveDataSO> WaveDataList { get; private set; } = new List<WaveDataSO>();

    public string GetStageFormat()
    {
        //스테이지 정보를 string 형태로 바꿔서 반환
        int world = (StageId / 100) % 100;
        int stage = StageId % 100;
        string info = world + "_" + stage;
        return info;
    }
}
