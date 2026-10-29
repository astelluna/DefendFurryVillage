using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;




[Serializable]
public class CharacterStat
{
    [Header("아래부터 인게임 정보")]
    [field: SerializeField] public int attack { get; private set; } = 0; //플레이어 기본 공격력
    [field: Header("크리확률 0~100까지 설정")]
    [field: SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률 0~100
    [field: Header("크리 데미지 배율 예시) 2.0 이렇게 설정(2배)")]
    [field: SerializeField] public float criticalDamageRate { get; private set; } = 0; //크리 데미지 배율 2.0 -> (2배 데미지)
    [field: Header("방어관통 0~100까지 설정 (float 가능)")]
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률 0~100; 35.5% -> DamageManager에서 0.355로 바꿔서 계산할거임

    public void SetData(int atk, int critPercent, float critRate, float armorpenet)
    {
        attack = atk;
        criticalPercent = critPercent;
        criticalDamageRate = critRate;
        armorPenetration = armorpenet;
    }

    public void SetZero()
    {
        //모든스텟을 0으로 만들고 반환
        attack = 0;
        criticalPercent = 0;
        criticalDamageRate = 0;
        armorPenetration = 0;
    }

    public void SetFromSum(CharacterStat a, CharacterStat b)
    {
        //두 객체를 합쳐서 자신에 더함 (total Stat을 위해 설정)
        attack = a.attack + b.attack;
        criticalPercent = a.criticalPercent + b.criticalPercent;
        criticalDamageRate = a.criticalDamageRate + b.criticalDamageRate;
        armorPenetration = a.armorPenetration + b.armorPenetration;
    }
}

public class Character : MonoBehaviour
{
    //캐릭터 인스턴스 정보
    public CharacterInstance CharacterInstanceData { get; private set; }
    //public CharacterDataSO CharacterDataSO { get; private set; }

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
        specialWeapon.OnWeaponShoot += SpecialWeaponShooted;

        //WeaponAttackStart();
    }
    private void OnDestroy()
    {
        basicWeapon.OnWeaponShoot -= characterAnimation.PlayAttackBasic;
        specialWeapon.OnWeaponShoot -= SpecialWeaponShooted;
    }

    private async Task Preload_Data()
    {
        //무기 데이터의 총알 프리팹 불러오기 
        var basicTask = AddressableManager.Instance.GetAssetAsync_Asset<Projectile>(CharacterInstanceData.characterData.BasicWeaponDataSO.ProjectileReferencePrefab);
        var specialTask = AddressableManager.Instance.GetAssetAsync_Asset<Projectile>(CharacterInstanceData.characterData.SpecialWeaponDataSO.ProjectileReferencePrefab);

        await Task.WhenAll(basicTask, specialTask);
        Projectile basicWeaponProjectile = basicTask.Result;
        Projectile specialWeaponProjectile = specialTask.Result;
        //무기 데이터 저장
        basicWeapon.SetWeaponInfo(CharacterInstanceData.characterData, CharacterInstanceData.characterData.BasicWeaponDataSO, basicWeaponProjectile);
        specialWeapon.SetWeaponInfo(CharacterInstanceData.characterData, CharacterInstanceData.characterData.SpecialWeaponDataSO, specialWeaponProjectile);
    }
    public void SetCharacterData(CharacterInstance data)
    {
        CharacterInstanceData = data;
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
    private void SpecialWeaponShooted()
    {
        characterAnimation.PlayAttackSpecial();
        if(CharacterInstanceData.characterData.AttackSound != null) { SoundManager.Instance.PlayVoice(CharacterInstanceData.characterData.AttackSound); }
        
    }
}
