using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/AugmentSpecialKnockBack")]
public class AugmentSpecialKnockBackSO : AugmentSpecialDataSO, IOnHit
{
    [SerializeField] float knockbackValue = 5;
    public override string GetDescription()
    {
        return $"넉백 효과를 추가합니다 ";
    }
    public void OnHit(WeaponRuntimeData runtimeData, Enemy enemy)
    {
        //몬스터가 넉백이 된다.
        enemy.KnockBack(knockbackValue);
    }
}
