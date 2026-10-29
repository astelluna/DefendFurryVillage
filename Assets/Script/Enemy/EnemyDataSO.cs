using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [field: SerializeField] public int attack { get; private set; } = 1;
    [field: SerializeField] public int defense { get; private set; }
    [field: SerializeField] public int maxHp { get; private set; } = 1;
    [field: SerializeField] public float attackRate { get; private set; } = 1.0f;
    [field: SerializeField] public float moveSpeed { get; private set; } = 1.0f;
    [field: SerializeField] public float penetration { get; private set; }
    [field: SerializeField] public int experience { get; private set; } = 1;
    [field: SerializeField] public float attackRange { get; private set; } = 0f; //기본0 -> 근접. 아니라면? 원거리 공격 개시

    [Header("몬스터 이미지 및 프리팹")]
    [field: SerializeField] public Sprite Portrite { get; private set; }
    [field: SerializeField] public AssetReferenceEnemy EnemyPrefab { get; private set; }
    [Header("사운드")]
    [field: SerializeField] public AudioClip DeadSound { get; private set; }
    [field: SerializeField] public AudioClip AttackSound { get; private set; }

    public bool CanRangedAttack()
    {
        if (attackRange > 0) return true;
        return false;
    }

}

