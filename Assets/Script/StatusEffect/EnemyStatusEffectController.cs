using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusEffectController : MonoBehaviour
{
    List<IStatusEffect> statusEffectList = new List<IStatusEffect>();
    List<IStatusEffect> removeEffectBuf = new List<IStatusEffect>();

    Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void AddStatusEffect(IStatusEffect effect)
    {
        effect.OnApply(enemy);
        statusEffectList.Add(effect);
    }

    private void Update()
    {
        //모든 상태이상에 대해서 지속시간 체크, 및 상태이상 계속 업데이트
        removeEffectBuf.Clear();
        foreach(var item in statusEffectList)
        {
            //효과 적용 및 인덱스 꼬이니까 임시 리프스에 지울 효과들 저장
            item.OnTick(enemy, Time.deltaTime);
            if(item.CheckDurationEnd())
            {
                removeEffectBuf.Add(item);
            }
        }
        foreach(var item in removeEffectBuf)
        {
            item.OnRemove(enemy);
            statusEffectList.Remove(item);
        }
    }
}
