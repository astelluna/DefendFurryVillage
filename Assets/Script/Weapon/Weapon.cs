using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public struct WeaponRuntimeData
{
    public int attackdamage;
    public float attackRate;
    public float bulletScale;
    public int plusAttackCount;
    public int maxPierce;
    public int criticalPercent;
    public float criticalDamgeRate;
    public float armorPenetrate;
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
public class Weapon : MonoBehaviour
{
    
    //런타임 데이터
    public WeaponRuntimeData runtimeData = new WeaponRuntimeData();
    List<AugmentSpecialDataSO> augmentSpecialDataSOList = new List<AugmentSpecialDataSO>();
    public IReadOnlyList<AugmentSpecialDataSO> AugmentSpecialDataSOList => augmentSpecialDataSOList;

    //발사 횟수가 2회이상일때 딜레이를 위해서 추가
    protected float shootDelay = 0.5f;
    public WaitForSeconds continuousFireDelay { get; private set; }



    //무기 정보들
    //[field: SerializeField] public Sprite WeaponIcon { get; protected set; }
    public WeaponDataSO WeaponDataSO { get; private set; }

    protected Coroutine shootCor;
    
    public float nextAttackTime { get; private set; }

    protected CharacterDataSO characterData;

    //증강 선택할때마다 아래 3개 리스트의 정보가 바뀜
    protected List<AugmentDataSO> augmentDataList = new List<AugmentDataSO>(); //현재 적용중인 증강 리스트

    protected List<AugmentDataSO> unUsedUpgradeAugmentDataList = new List<AugmentDataSO>(); //증강 업그레이드 선택하면 여기에 들어옴
    protected List<AugmentDataSO> usedUpgradeAugmentDataList = new List<AugmentDataSO>(); //증강 업그레이드 선택하면 여기에 들어옴

    //무기 발사했다는 이벤트
    public event Action OnWeaponShoot;

    //플레이어가 가지고있는 Weapon프리팹(어드레서블로 되어있고 캐릭터 불러올때 프리팹도 불러와서 설정)
    public Projectile ProjectilePrefab { get; private set; }
    //Projectile specialWeaponProjectile;

    public void InvokeWeaponShootEvent()
    {
        OnWeaponShoot?.Invoke();
    }

    public async Task Init()
    {
        unUsedUpgradeAugmentDataList = new List<AugmentDataSO>(WeaponDataSO.ExistUpgradeAugmentDataList);



        //weaponDataTask = AddressablePreloadManager.Instance.GetAsync<WeaponDataSO>(weaponDataAddressable);
        //weaponDataSO = await weaponDataTask;
    }
    public void StartWeapon()
    {
        SetWeaponCoolTime();
        if (shootCor == null)
        {
            shootCor = StartCoroutine(WeaponDataSO.weaponExecuteSO.ShootWeapon(this));
        }
    }

    protected virtual void OnDestroy()
    {
        if (shootCor != null)
        {
            StopCoroutine(shootCor);
            shootCor = null;
        }
    }

    private async Task LoadAddressable_basic()
    {
        //프리로드 하는 방식이 아니라 수동 로드할때는 이렇게 하자
        //if (weaponDataSO != null) return;
        //AsyncOperationHandle<WeaponDataSO> weaponDataHandle = Addressables.LoadAssetAsync<WeaponDataSO>(weaponDataAddressable);
        //weaponDataSO = weaponDataHandle.WaitForCompletion();
        //weaponDataSO = await weaponDataHandle.Task;
        //Addressables.Release(weaponDataHandle);
    }

    private async Task LoadAddressableㅁㄴㅁㄶㅁㄶㄴ()
    {
        //어드레서블 프리로드한 곳에서 어드레서블 정보를 가져온다.
        //if (weaponDataSO != null) return;
        //weaponDataSO = await AddressablePreloadManager.Instance.GetAsync<WeaponDataSO>(weaponDataAddressable);
    }

    //증강이 선택되면 현재 무기의 런타임 정보를 갱신한다.
    public void UpdateWeaponData()
    {
        float damagePercent = 0;
        float ratePercent = 0;
        float bulletSizePercent = 0;
        int attackCount = 0;
        int pierceCount = 0;


        //증강을 basicAugmentDataSOList + usedUpgradeAugmentDataList로 변경해야함.
        augmentDataList.Clear();
        augmentSpecialDataSOList.Clear();
        foreach (var item in WeaponDataSO.BasicAugmentDataSOList)
        {
            augmentDataList.Add(item);
        }
        foreach(var item in usedUpgradeAugmentDataList)
        {
            augmentDataList.Add(item);
        }
        foreach (AugmentDataSO item in augmentDataList)
        {
            damagePercent += item.addAttackDamage;
            ratePercent += item.reduceAttackRate;
            bulletSizePercent += item.addProjectileScale;
            attackCount += item.addAttackCount;
            pierceCount += item.addPierceCount;

            if (item.AugmentSpecialDataSOList != null && item.AugmentSpecialDataSOList.Count > 0)
            {
                augmentSpecialDataSOList.AddRange(item.AugmentSpecialDataSOList);
            }
        }

        //runtimeData = new WeaponRuntimeData();
        runtimeData.attackdamage = characterData.CharacterBaseStat.attack + WeaponDataSO.baseDamage + (int)(WeaponDataSO.baseDamage * damagePercent);
        runtimeData.attackRate = WeaponDataSO.baseAttackRate + (WeaponDataSO.baseAttackRate * ratePercent);
        runtimeData.bulletScale = 1f + (1f * bulletSizePercent);
        runtimeData.plusAttackCount = attackCount;
        runtimeData.maxPierce = 1 + pierceCount;
        runtimeData.criticalPercent = characterData.CharacterBaseStat.criticalPercent;
        runtimeData.criticalDamgeRate = characterData.CharacterBaseStat.criticalDamageRate;
        runtimeData.armorPenetrate = characterData.CharacterBaseStat.armorPenetration;

        //후처리
        if (runtimeData.attackdamage < 1) runtimeData.attackdamage = 1;
        runtimeData.attackRate = Mathf.Round(runtimeData.attackRate * 100f) / 100f;
        if (runtimeData.bulletScale < 0.1) runtimeData.bulletScale = 0.1f;
        if (runtimeData.plusAttackCount < 1) runtimeData.plusAttackCount = 1;
    }
    public AugmentDataSO GetRandomAugmentData()
    {
        if (unUsedUpgradeAugmentDataList.Count == 0) return null;
        int index = UnityEngine.Random.Range(0, unUsedUpgradeAugmentDataList.Count);
        AugmentDataSO dataSO = unUsedUpgradeAugmentDataList[index];
        unUsedUpgradeAugmentDataList.Remove(dataSO);
        usedUpgradeAugmentDataList.Add(dataSO);
        return dataSO;
    }
    public bool HaveAugmentUpgrade()
    {
        if (unUsedUpgradeAugmentDataList.Count == 0) return false;
        return true;
    }
    public void ReturnAugment(AugmentDataSO augment)
    {
        //증강 미선택되어서 다시 돌려보냄
        unUsedUpgradeAugmentDataList.Add(augment);
        usedUpgradeAugmentDataList.Remove(augment);
    }
    public void SetWeaponInfo(CharacterDataSO ChaData, WeaponDataSO weaponDataSO, Projectile projectile)
    {
        WeaponDataSO = weaponDataSO;
        characterData = ChaData;
        continuousFireDelay = new WaitForSeconds(shootDelay);
        ProjectilePrefab = projectile;
        unUsedUpgradeAugmentDataList = new List<AugmentDataSO>(WeaponDataSO.ExistUpgradeAugmentDataList);
        UpdateWeaponData();
        
    }

    public void SetWeaponCoolTime()
    {
        nextAttackTime = Time.time + WeaponDataSO.baseAttackRate;
    }

    //무기 공격 동작 코루틴은 자식이 알아서 하도록 하자.
    //protected abstract IEnumerator ShootWeapon();
    public virtual Transform FindExistTransform(List<Transform> enemyTransform)
    {
        foreach (var item in enemyTransform)
        {
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }
}
