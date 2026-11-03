using System.Collections.Generic;
using System.Text;
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

    public string GetDataDescription()
    {
        StringBuilder sb = new StringBuilder();
        if (addAttackDamage != 0) sb.Append($"공격력을 {addAttackDamage}% ");
        if (reduceAttackRate != 0) sb.Append($"쿨타임을 {reduceAttackRate}% ");
        if (addProjectileScale != 0) sb.Append($"투사체 크기를 {addProjectileScale}% ");
        if (addAttackCount != 0) sb.Append($"공격 횟수를 {addAttackCount} ");
        if (addPierceCount != 0) sb.Append($"관통 횟수를 {addPierceCount} ");
        if(haveBasicAgmentData())
        {
            sb.Append("추가합니다");
        }
        //특수 스킬들
        foreach (var item in augmentSpecialDataSOList)
        {
            sb.Append(item.GetDescription());
        }
        
        return sb.ToString();
    }
    private bool haveBasicAgmentData()
    {
        if(addAttackDamage != 0 || reduceAttackRate != 0 || addProjectileScale !=0 || addAttackCount != 0 || addPierceCount != 0)
        {
            return true;
        }
        return false;
    }
}


