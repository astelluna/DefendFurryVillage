using UnityEngine;
public class BurnStatusEffect : StatusEffect
{
    float tickTime = 1f; //몇초마다 한번씩 발동하는가
    float curTickTime = 0f;

    public BurnStatusEffect(int value, float dur, GameObject vfx) : base(value,dur,vfx) 
    {
        curDurationTime = 0;
    }
    public override void OnTick(Enemy enemy, float time)
    {
        curDurationTime += time;
        curTickTime += time;
        if(curTickTime >= tickTime)
        {
            curTickTime = 0;
            enemy.TakeDamage(amount);
        }
    }
}
