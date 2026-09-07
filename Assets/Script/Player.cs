using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    [field: SerializeField] public int attack { get; private set; } = 10;
    [field:SerializeField] public float criticalRate { get; private set; } = 0; //크리 확률
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
    PlayerStat playerStat;
    //무기 스텟? -> SO로 빼야할듯
    [SerializeField] float attackRate = 1f; //공격 속도
    [SerializeField] float weaponRange = 5f; //무기 사거리
    [SerializeField] float specialWeaponRange = 5f; //특수무기 사거리
    [SerializeField] float specialAttackRate = 5f; //공격 속도


    //이게 일반 공격, 특수 공격
    //#2. Weapon 클래스
    [SerializeField] Weapon basicWeapon;
    [SerializeField] Weapon specialWeapon;

    //캐릭터의 초상화 sprite 
    [field: SerializeField] public Sprite Portrait { get; private set; }


    private void Start()
    {
        basicWeapon.SetWeaponInfo(this.playerStat);
        specialWeapon.SetWeaponInfo(this.playerStat);

        AugmentManager.Instance.AddPlayer(this);
    }


    //TODO. 추후 데미지 계산 -> 플레이어에서 하는게 아니라 무기에서 해야할수도? 
    public float CalcDamage( int armor)
    {
        //현재 공격이 치명타인지 확인.
        //bool isCritical = false;
        //float damage = attack;
        //float percent = Random.Range(0, 100);
        //if(percent >= criticalRate)
        //{
        //    //크리
        //    isCritical = true;
        //}
        //
        //if(isCritical)
        //{
        //    damage *= criticalDamageRate;
        //}
        //
        ////아머를 고려해서 최종 데미지 계산
        //float result = damage * (100 / (100 + armor));
        //return result;
        return 0;
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
