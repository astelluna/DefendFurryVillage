using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    [field: SerializeField] public int attack { get; private set; } = 10;
    [field:SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률
    [field:SerializeField] public float criticalDamageRate { get; private set; } = 2; //크리 데미지 배율 (2배)
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률
}

public class WeaponAugment
{
    public Weapon weapon;
    public AugmentDataSO dataSO;
    public event Action<WeaponAugment> OnSelected;
    public WeaponAugment(Weapon wp, AugmentDataSO data)
    {
        weapon = wp;
        dataSO = data;
    }

    public void SelectedEventInvoke(WeaponAugment augment)
    {
        OnSelected?.Invoke(augment);
    }
    
}

public class Player : MonoBehaviour
{
    //일단 플레이어에게 필요한거 정의를 다 해보자
    //TODO. (일단 직렬화 시킴. 추후에는 SO로 캐릭터 데이터 받아서 하거나. 그런식으로 처리해보자)
    //캐릭터 기본 스텟
    [SerializeField] PlayerDataSO playerDataSO;

    //플레이어가 가지고있는 Weapon
    [SerializeField] Weapon basicWeapon;
    [SerializeField] Weapon specialWeapon;

    //캐릭터의 초상화 sprite 
    [field: SerializeField] public Sprite Portrait { get; private set; }


    private void Awake()
    {
        PlayerStat stat = new PlayerStat();
    }

    private void Start()
    {
        basicWeapon.SetWeaponInfo(this.playerDataSO);
        specialWeapon.SetWeaponInfo(this.playerDataSO);

        AugmentManager.Instance.AddPlayer(this);
    }

    public WeaponAugment GetAugment()
    {
        List<Weapon> haveUpgradeWeaponList = new List<Weapon>();
        if(basicWeapon.HaveAugmentUpgrade())
        {
            haveUpgradeWeaponList.Add(basicWeapon);
        }
        if(specialWeapon.HaveAugmentUpgrade())
        {
            haveUpgradeWeaponList.Add(specialWeapon);
        }

        if (haveUpgradeWeaponList.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, haveUpgradeWeaponList.Count);
        Weapon curWeapon = haveUpgradeWeaponList[index];
        AugmentDataSO dataSO = curWeapon.GetRandomAugmentData();
        WeaponAugment weaponAugment = new WeaponAugment(curWeapon, dataSO);
        return weaponAugment;

    }
    public bool HaveRemainWeaponAugment()
    {
        if(basicWeapon.HaveAugmentUpgrade() || specialWeapon.HaveAugmentUpgrade() ) { return true; }
        return false;
    }

    
}
