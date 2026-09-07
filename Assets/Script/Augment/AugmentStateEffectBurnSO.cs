using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/AugmentStatusEffectBurn")]
public class AugmentStateEffectBurnSO : AugmentSpecialDataSO, IOnHit
{
    [field: SerializeField] public int amount { get; private set; }//몇초뒤에 데미지를 주는가?
    [field: SerializeField] public int duration { get; private set; }
    [field: SerializeField] public GameObject VfxPrefab { get; private set; }
    public override string GetDescription()
    {
        return $"화상 효과가 적용됩니다 ";
    }
    public void OnHit(WeaponRuntimeData runtimeData, Enemy enemy)
    {
        BurnStatusEffect burnStatusEffect = new BurnStatusEffect(amount, duration, VfxPrefab);
        enemy.AddStatusEffect(burnStatusEffect);
        MyLog.Log("화상 적용: ");

    }
}
