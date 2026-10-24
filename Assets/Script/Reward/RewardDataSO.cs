using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardItem
{
    //보상으로 장비를 count개 만큼 준다.
    [field:SerializeField] public ItemDataSO itemDataSO { get; private set; }
    [field: SerializeField] public int Count { get; private set; }
}
[CreateAssetMenu(menuName = "Reward/RewardDataSO")]
public class RewardDataSO : ScriptableObject
{
    //돈과 List에 표시된 장비들을 n개씩 준다.
    [field: SerializeField] public long RewardMoney { get; private set; }
    [field: SerializeField] public List<RewardItem> RewardItemList { get; private set; }
}
