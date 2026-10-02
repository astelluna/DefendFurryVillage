using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Weapon/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    //public string weaponName;
    //[field:SerializeField] public GameObject bulletPrefab { get; private set; } //총알 종류
    [field: SerializeField] public Sprite WeaponIcon { get; private set; }
    [field: SerializeField] public float baseRange { get; private set; }
    [field:SerializeField] public float baseAttackRate { get; private set; }
    [field:SerializeField] public int baseDamage { get; private set; }
    [field: SerializeField] public string WeaponDescription { get; private set;} //이 무기는 어떤 무기인지 설명하는 텍스트

    [Header("무기 실행정보 SO")]
    [field: SerializeField] public WeaponExecuteSO weaponExecuteSO { get; private set; }
    [Header("증강 정보 설정")]
    [Header("기본적으로 적용되는 증강 리스트")]
    //기본적으로 적용되는 증강 리스트
    [field: SerializeField] private List<AugmentDataSO> basicAugmentDataSOList = new List<AugmentDataSO>();
    public IReadOnlyList<AugmentDataSO> BasicAugmentDataSOList => basicAugmentDataSOList;

    [Header("레벨업 시 증강 리스트")]
    //레벨업 시 선택할수 있는 증강 리스트 (랜덤으로 하나 선택됨)
    [field: SerializeField] private List<AugmentDataSO> existUpgradeAugmentDataList = new List<AugmentDataSO>();
    public IReadOnlyList<AugmentDataSO> ExistUpgradeAugmentDataList => existUpgradeAugmentDataList;

    //총알 프리팹 종류
    //[field: SerializeField] public Projectile ProjectilePrefab { get; private set; } //총알 종류
    [Header("총알프리팹-어드레서블")]
    [field: SerializeField] public AssetReferenceProjectile ProjectileReferencePrefab { get; private set; }

}
