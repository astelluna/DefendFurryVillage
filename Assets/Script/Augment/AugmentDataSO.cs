using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/AugmentDataSO")]
public class AugmentDataSO : ScriptableObject
{
    [field: SerializeField] public float addAttackDamage { get; private set; } //공격 데미지
    [field: SerializeField] public float reduceAttackRate { get; private set; } //공격 주기
    [field: SerializeField] public float addProjectileScale { get; private set; } //발사체 사이즈(총알크기)
    [field: SerializeField] public int addAttackCount { get; private set; } //추가 공격 횟수
    [field: SerializeField] public int addPierceCount { get; private set; } //추가 관통 횟수
    [SerializeField] List<AugmentSpecialDataSO> augmentSpecialDataSOList;
    public IReadOnlyList<AugmentSpecialDataSO> AugmentSpecialDataSOList => augmentSpecialDataSOList;
}


