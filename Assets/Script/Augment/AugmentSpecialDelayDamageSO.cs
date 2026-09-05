using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/AugmentSpecialDelay")]
public class AugmentSpecialDelayDamageSO : AugmentSpecialDataSO, IOnHit
{
    [field:SerializeField] public float delayTime { get; private set; } = 3.0f; //몇초뒤에 데미지를 주는가?
    [field: SerializeField] public float damageRatio { get; private set; } = 0.5f; //데미지 비율 - 원본 총알 데미지의 몇퍼센트 데미지를 주는가

    public void OnHit(WeaponRuntimeData weaponRuntimeData, Enemy enemy)
    {
        int dmg = (int)(weaponRuntimeData.attackdamage * damageRatio);
        enemy.OnHitDelay(dmg, delayTime);
    }
}
