using UnityEngine;

public interface IStatusEffect
{
    public void OnApply(Enemy enemy);
    public void OnTick(Enemy enemy, float time);
    public void OnRemove(Enemy enemy);
    public bool CheckDurationEnd();
    public GameObject VfxPrefab { get; }

}

public abstract class StatusEffect : IStatusEffect
{
    protected int amount;
    protected float duration;
    protected float curDurationTime;
    public GameObject VfxPrefab { get; private set; }

    public StatusEffect(int value, float dur, GameObject vfx)
    {
        amount = value;
        duration = dur;
        VfxPrefab = vfx;

        //초기화 
        curDurationTime = 0;
    }

    public void OnApply(Enemy enemy)
    {
        enemy.RegisterEffect(VfxPrefab);
    }

    public void OnRemove(Enemy enemy)
    {
        enemy.RemoveEffect(VfxPrefab);
    }
    //지속시간이 끝났는가?
    public bool CheckDurationEnd()
    {
        if (curDurationTime >= duration) return true;
        return false;
    }
    public abstract void OnTick(Enemy enemy, float time);

    
}

