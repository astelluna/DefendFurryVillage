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

    //행동을 추가/삭제할때 해당 상태이상이 특별히 해야할행동이 있다면 여기에서 추가
    public abstract void OnApplySpecial(Enemy enemy);
    public abstract void OnTickSpecial(Enemy enemy, float time); 
    public abstract void OnRemoveSpecial(Enemy enemy);
    

    //기본 추가,삭제, 틱 동작들
    public void OnApply(Enemy enemy)
    {
        enemy.RegisterEffect(VfxPrefab);
        OnApplySpecial(enemy);
    }

    public void OnRemove(Enemy enemy)
    {
        enemy.RemoveEffect(VfxPrefab);
        OnRemoveSpecial(enemy);

    }
    //지속시간이 끝났는가?
    public bool CheckDurationEnd()
    {
        if (curDurationTime >= duration) return true;
        return false;
    }
    public void OnTick(Enemy enemy, float time)
    {
        curDurationTime += time;
        OnTickSpecial(enemy, time);
    }

    
}

