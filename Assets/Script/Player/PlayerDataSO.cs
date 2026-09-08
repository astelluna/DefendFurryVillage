using UnityEngine;
[CreateAssetMenu(menuName = "Player/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [field: SerializeField] public int attack { get; private set; } = 10; //플레이어 기본 공격력
    [field: Header("크리확률 0~100까지 설정")]
    [field: SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률 0~100
    [field: Header("크리 데미지 배율 예시) 2.0 이렇게 설정(2배)")]
    [field: SerializeField] public float criticalDamageRate { get; private set; } = 2.0f; //크리 데미지 배율 2.0 -> (2배 데미지)
    [field:Header("방어관통 0~100까지 설정 (float 가능)")]
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률 0~100; 35.5% -> DamageManager에서 0.355로 바꿔서 계산할거임

}
