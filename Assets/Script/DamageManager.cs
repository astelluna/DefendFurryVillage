using UnityEngine;
public class AttackerInfo
{
    //공격자 능력치
    public int attackerAttack { get; private set; }
    public int attackerCriticalPercent { get; private set; } //15 = 15% 확률
    public float attackerCriticalRate { get; private set; } // 2.0이면 2배, 2.5면 2.5배로 적용
    public float attackerArmorPenetrate { get; private set; } // 30f -> 30%관통
    //피격자 능력치
    //public int targetDefense { get; private set; } //피격자 방어력


    public void SetAttackerInfo(int atk, int critPercent, float critRate, float armorPenet)
    {
        attackerAttack = atk;
        attackerCriticalPercent = critPercent;
        attackerCriticalRate = critRate;
        attackerArmorPenetrate = armorPenet;
    }
}

public class DefenserInfo
{
    //피격자 능력치
    public int targetDefense { get; private set; } //피격자 방어력

    public void SetDefenserInfo(int def)
    {
        targetDefense = def;
    }
}


public class DamageManager : MonoBehaviour
{
    //데미지 계산을 해주는 매니저
    public static DamageManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    public int CalcDamage(AttackerInfo info, DefenserInfo defenserInfo)
    {
        //공격자의 정보와 피격자의 방어력을 넘긴다.

        //일단 크리티컬 확률에 따른 크리인지 아닌지 체크부터.
        int curAttackPoint = info.attackerAttack;
        if(info.attackerCriticalPercent > 0)
        {
            float percent = Random.Range(0, 100) + 1;  // 1~ 100까지
            if(percent <= info.attackerCriticalPercent)
            {
                //크리티컬
                curAttackPoint = (int)(curAttackPoint * info.attackerCriticalRate);
            }
        }
        //데미지 계산 (방관은 비율 감소식으로 한다)
        float attackPenetrate = info.attackerArmorPenetrate / 100f; //실제 사용할때는 1을 최대값으로 100% -> 1;
        if (attackPenetrate >= 1f) attackPenetrate = 1f; //예외처리
        float effectiveDef = defenserInfo.targetDefense * (1f - attackPenetrate);
        effectiveDef = Mathf.Max(effectiveDef, 0f); //예외 처리
        int damage = (int)(curAttackPoint * (100f / (100f + effectiveDef)));
        damage = Mathf.Max(damage, 1);
        MyLog.Log($"계산된 데미지: {damage}");
        return damage;
    

    }

}