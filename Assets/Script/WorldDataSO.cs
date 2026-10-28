using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "World/WorldDataSO")]
public class WorldDataSO : ScriptableObject
{
    [field:SerializeField] public string worldName { get; private set; } //월드 이름 (화면에 표시됨)
    [field: SerializeField] public List<StageDataSO> StageDataList { get; private set; } = new List<StageDataSO>(); //해당 월드의 스테이지 목록
}
