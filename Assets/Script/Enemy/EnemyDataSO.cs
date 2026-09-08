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


}

