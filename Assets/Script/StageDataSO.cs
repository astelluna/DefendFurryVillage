using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "StageWave/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    [field: SerializeField] public List<WaveDataSO> WaveDataList { get; private set; } = new List<WaveDataSO>();
}
