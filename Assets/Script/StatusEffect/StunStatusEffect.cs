using UnityEngine;

public class StunStatusEffect : StatusEffect
{
    public StunStatusEffect(int value, float dur, GameObject vfx) : base(value, dur, vfx)
    {
    }

    public override void OnApplySpecial(Enemy enemy)
    {
        enemy.ApplyStunState(this);
    }

    public override void OnRemoveSpecial(Enemy enemy)
    {
        enemy.RemoveStunState(this);
    }

    public override void OnTickSpecial(Enemy enemy, float time)
    {
    }
}
