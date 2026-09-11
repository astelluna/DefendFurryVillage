using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    //public string weaponName;
    //[field:SerializeField] public GameObject bulletPrefab { get; private set; } //총알 종류
    [field: SerializeField] public float baseRange { get; private set; }
    [field:SerializeField] public float baseAttackRate { get; private set; }
    [field:SerializeField] public int baseDamage { get; private set; }

}
