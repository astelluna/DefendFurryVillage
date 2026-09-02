using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    //public string weaponName;
    public GameObject bulletPrefab; //총알 종류
    public float baseRange;
    public float baseAttackRate;
    public int baseDamage;

}
