using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterStat
{
    [field: SerializeField] public int attack { get; private set; } = 10;
    [field:SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률
    [field:SerializeField] public float criticalDamageRate { get; private set; } = 2; //크리 데미지 배율 (2배)
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률
}

public class Character : MonoBehaviour
{
    //캐릭터 기본 스텟
    //[SerializeField] AssetReferenceT<CharacterDataSO> characterDataAddressable; asdasf설정 다시해야함
    public CharacterDataSO CharacterDataSO { get; private set; }

    //플레이어가 가지고있는 Weapon
    [SerializeField] Weapon basicWeapon;
    [SerializeField] Weapon specialWeapon;



    //캐릭터의 초상화 sprite 
    //[field: SerializeField] public Sprite Portrait { get; private set; }

    public event Action<Character> OnStartCharacterSpawn;

    //애니메이션
    [SerializeField] CharacterAnimation characterAnimation;

    private void Start()
    {
        basicWeapon.OnWeaponShoot += characterAnimation.PlayAttackBasic;
        specialWeapon.OnWeaponShoot += characterAnimation.PlayAttackSpecial;

        //WeaponAttackStart();
    }
    private void OnDestroy()
    {
        basicWeapon.OnWeaponShoot -= characterAnimation.PlayAttackBasic;
        specialWeapon.OnWeaponShoot -= characterAnimation.PlayAttackSpecial;
    }

    private async Task Preload_Data()
    {
        //무기 데이터의 총알 프리팹 불러오기 
        var basicTask = AddressableManager.Instance.GetAssetAsync_Asset<Projectile>(CharacterDataSO.BasicWeaponDataSO.ProjectileReferencePrefab);
        var specialTask = AddressableManager.Instance.GetAssetAsync_Asset<Projectile>(CharacterDataSO.SpecialWeaponDataSO.ProjectileReferencePrefab);
        //var basicTask = CharacterManager.Instance.AddressableLoadFunc<Projectile>(CharacterDataSO.BasicWeaponDataSO.ProjectileReferencePrefab);
        //var specialTask = CharacterManager.Instance.AddressableLoadFunc<Projectile>(CharacterDataSO.SpecialWeaponDataSO.ProjectileReferencePrefab);

        await Task.WhenAll(basicTask, specialTask);
        Projectile basicWeaponProjectile = basicTask.Result;
        Projectile specialWeaponProjectile = specialTask.Result;
        //무기 데이터 저장
        basicWeapon.SetWeaponInfo(this.CharacterDataSO, this.CharacterDataSO.BasicWeaponDataSO, basicWeaponProjectile);
        specialWeapon.SetWeaponInfo(this.CharacterDataSO, this.CharacterDataSO.SpecialWeaponDataSO, specialWeaponProjectile);
    }
    public void SetCharacterSO(CharacterDataSO dataSO)
    {
        CharacterDataSO = dataSO;
    }
    public async Task Init()
    {
        //실제 플레이어 소환되고 처리해야할 Init 동작
        await Preload_Data();
        
    }
    public void WeaponAttackStart()
    {
        //공격 명령
        basicWeapon.StartWeapon();
        specialWeapon.StartWeapon();
    }

    public void CharacterSpawnEventInvoke()
    {
        
        OnStartCharacterSpawn?.Invoke(this);
    }

    public bool CompareCharacterSame(Character otherCharacter)
    {
        if (this.CharacterDataSO == otherCharacter.CharacterDataSO) return true;
        return false;
    }

    public void MakeWeaponProjectilePool()
    {
        //스포너에 생성해달라는 요청을 해줌
        ProjectileSpawner.Instance.MakeProjectile(basicWeapon.ProjectilePrefab);
        ProjectileSpawner.Instance.MakeProjectile(specialWeapon.ProjectilePrefab);
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

    public void CharacterActive(bool isActive)
    {
        this.gameObject.SetActive(isActive);

        if(isActive)
        {
            WeaponAttackStart();
        }
    }

    #region 리소스 가져오기
    public Sprite GetBasicWeaponIcon()
    {
        //return basicWeapon.weaponDataSO.WeaponIcon;
        return null;
    }
    public string GetBasicWeaponDesc()
    {
        //return basicWeapon.weaponDataSO.WeaponDescription;
        return null;
    }
    public Sprite GetSpecialWeaponIcon()
    {
        //return specialWeapon.weaponDataSO.WeaponIcon;
        return null;
    }
    public string GetSpecialWeaponDesc()
    {
        //return specialWeapon.weaponDataSO.WeaponDescription;
        return null;
    }
    public CharacterDataSO GetCharacterDataSO()
    {
        //return CharacterDataSO;
        return null;
    }
    #endregion

}
