using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Augment/AugmentStatusEffectStun")]
public class AugmentStateEffectStunSO : AugmentSpecialDataSO, IOnHit
{
    [field: SerializeField] float stunTime = 3f;
    [field: SerializeField] public GameObject VfxPrefab { get; private set; }
    public override string GetDescription()
    {
        return $"스턴 효과 적용됩니다";
    }

    public void OnHit(WeaponRuntimeData runtimeData, Enemy enemy)
    {

        StunStatusEffect stunStatusEffect = new StunStatusEffect(0, stunTime, VfxPrefab);
        enemy.AddStatusEffect(stunStatusEffect);
        MyLog.Log("스턴 적용");
    }
}
