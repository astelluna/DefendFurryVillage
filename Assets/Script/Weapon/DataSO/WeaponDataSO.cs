using UnityEngine;

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

}
