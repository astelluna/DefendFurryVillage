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

    public event Action<CharacterDataSO> OnStartCharacterSpawn;

    //애니메이션
    [SerializeField] CharacterAnimation characterAnimation;

    private void Start()
    {
        basicWeapon.OnWeaponShoot += characterAnimation.PlayAttackBasic;
        specialWeapon.OnWeaponShoot += characterAnimation.PlayAttackSpecial;
    }
    private void OnDestroy()
    {
        basicWeapon.OnWeaponShoot -= characterAnimation.PlayAttackBasic;
        specialWeapon.OnWeaponShoot -= characterAnimation.PlayAttackSpecial;
    }

    private async Task Preload_Data()
    {
        //if (characterDataSO == null)
        //{
        //    await LoadAddressable();
        //}
        await basicWeapon.Init();
        await specialWeapon.Init();
        basicWeapon.SetWeaponInfo(this.CharacterDataSO);
        specialWeapon.SetWeaponInfo(this.CharacterDataSO);
    }
    public void SetCharacterSO(CharacterDataSO dataSO)
    {
        CharacterDataSO = dataSO;
    }
    public async Task Init_Preload()
    {
        
        //원본 프리팹에서 어드레서블로 되어있어서 정보만 Load하기 위한 메서드
        await Preload_Data();
    }
    public async Task Init()
    {
        //실제 플레이어 소환되고 처리해야할 Init 동작
        await Preload_Data();
        WeaponAttackStart();
    }
    public void WeaponAttackStart()
    {
        //공격 명령
        basicWeapon.StartWeapon();
        specialWeapon.StartWeapon();
    }

    public async Task LoadAddressable_basic()
    {
        //AsyncOperationHandle<CharacterDataSO> characterDataHandle = Addressables.LoadAssetAsync<CharacterDataSO>(characterDataAddressable);
        //characterDataSO = await characterDataHandle.Task;
        //Addressables.Release(characterDataHandle); //사용하고 바로 풀어주기 (다른곳에서 안씀)
    }
    public async Task LoadAddressable()
    {
        //characterDataSO = await AddressablePreloadManager.Instance.GetAsync<CharacterDataSO>(characterDataAddressable);
    }

    public void CharacterSpawnEventInvoke()
    {
        OnStartCharacterSpawn?.Invoke(CharacterDataSO);
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


    #region 리소스 가져오기
    public Sprite GetBasicWeaponIcon()
    {
        return basicWeapon.weaponDataSO.WeaponIcon;
    }
    public string GetBasicWeaponDesc()
    {
        return basicWeapon.weaponDataSO.WeaponDescription;
    }
    public Sprite GetSpecialWeaponIcon()
    {
        return specialWeapon.weaponDataSO.WeaponIcon;
    }
    public string GetSpecialWeaponDesc()
    {
        return specialWeapon.weaponDataSO.WeaponDescription;
    }
    public CharacterDataSO GetCharacterDataSO()
    {
        return CharacterDataSO;
    }
    #endregion

}
