using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "StageWave/WaveDataSO")]
public class WaveDataSO : ScriptableObject
{
    [field: SerializeField] public List<Enemy> EnemieList { get; private set; } = new List<Enemy>();

}
