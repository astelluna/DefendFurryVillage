using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerStat
{
    [field: SerializeField] public int attack { get; private set; } = 10;
    [field:SerializeField] public int criticalPercent { get; private set; } = 0; //크리 확률
    [field:SerializeField] public float criticalDamageRate { get; private set; } = 2; //크리 데미지 배율 (2배)
    [field: SerializeField] public float armorPenetration { get; private set; } = 0; //방어 관통률
}

public class Player : MonoBehaviour
{
    //캐릭터 기본 스텟
    [SerializeField] AssetReferenceT<PlayerDataSO> playerDataAddressable;
    PlayerDataSO playerDataSO;

    //플레이어가 가지고있는 Weapon
    [SerializeField] Weapon basicWeapon;
    [SerializeField] Weapon specialWeapon;

    //캐릭터의 초상화 sprite 
    [field: SerializeField] public Sprite Portrait { get; private set; }

    public event Action<Player> OnStartPlayerSpawn;

    private async Task Preload_Data()
    {
        if (playerDataSO == null)
        {
            await LoadAddressable();
        }
        await basicWeapon.Init();
        await specialWeapon.Init();
        basicWeapon.SetWeaponInfo(this.playerDataSO);
        specialWeapon.SetWeaponInfo(this.playerDataSO);
    }
    public async Task Init_Preload()
    {
        //원본 프리팹에서 어드레서블로 되어있어서 정보만 Load하기 위한 메서드
        await Preload_Data();
    }
    public async Task Init()
    {
        //실제 인게임에서 원본 프리팹을 통해 Instantiate하고. 그 정보로 Weapon 발사 설정까지 진행
        await Preload_Data();
        //공격 명령
        basicWeapon.StartWeapon();
        specialWeapon.StartWeapon();
    }

    public async Task LoadAddressable()
    {
        AsyncOperationHandle<PlayerDataSO> playerDataHandle = Addressables.LoadAssetAsync<PlayerDataSO>(playerDataAddressable);
        playerDataSO = await playerDataHandle.Task;
        Addressables.Release(playerDataHandle); //사용하고 바로 풀어주기 (다른곳에서 안씀)
    }

    public void PlayerSpawnEventInvoke()
    {
        OnStartPlayerSpawn?.Invoke(this);
    }

    public bool ComparePlayerSame(Player otherPlayer)
    {
        if (this.playerDataSO == otherPlayer.playerDataSO) return true;
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
    public Sprite GetSpecialWeaponIcon()
    {
        return specialWeapon.WeaponIcon;
    }
    public string GetSpecialWeaponDesc()
    {
        return specialWeapon.weaponDataSO.WeaponDescription;
    }

}
