using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Augment/AugmentSpecialSpread")]
public class AugmentSpecialSpreadProjectileSO : AugmentSpecialDataSO, IOnHit
{
    [SerializeField] Projectile projectilePrefab;

    List<Vector3> directionList;
    public override string GetDescription()
    {
        //주변  8개 방향으로 총알이 생성되어서 날라간다.
        if (directionList == null || directionList.Count == 0)
        {
            Init();
        }
        return $"주변 {directionList.Count}의 방향으로 추가 투사체가 발사됩니다 ";
    }
    public void OnHit(WeaponRuntimeData weaponRuntimeData, Enemy enemy)
    {
        //주변  8개 방향으로 총알이 생성되어서 날라간다.
        if(directionList == null || directionList.Count == 0)
        {
            Init();
        }
        //방향으로 생성해서 설정
        for(int i=0; i< directionList.Count; i++)
        {
            //Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
            //만약 흩날리는 총알이 오브젝트풀에 등록 안되있다면 등록시키고, 꺼내옴
            ProjectileSpawner.Instance.MakeProjectile(projectilePrefab);
            Projectile projectile = ProjectileSpawner.Instance.GetPrefabByPool(enemy.transform, projectilePrefab);
            projectile.gameObject.SetActive(false);
            
            //그냥 해당 enemy무시처리하지말고 기본 2명이상 관통하는거로 하자
            if (weaponRuntimeData.maxPierce <= 1)
            {
                weaponRuntimeData.maxPierce = 30;
            }
            projectile.SetProjectileDataAndInit(weaponRuntimeData, null, null); //null로 해서 다른 특수효과 재귀적으로 호출되지않게 해야할듯
            projectile.SetDirectionToDirection(enemy.transform.position, directionList[i]);
            projectile.Shoot();
            projectile.gameObject.SetActive(true);
            


            //Bullet bullet = Instantiate(projectilePrefab).GetComponent<Bullet>();
            //bullet.SetDirectionToDirection(enemy.transform.position, directionList[i]);
            //bullet.SetProjectileData(weaponRuntimeData, null);
            //bullet.SetBulletHitCount(2); //그냥 해당 enemy무시처리하지말고 기본 2명이상 관통하는거로 하자
        }

    }

    private void Init()
    {
        //주변 8개 방향으로 퍼지는 총알의 좌표를 등록한다
        directionList = new List<Vector3>();
        directionList.Add(new Vector3(-1, 0, -1));
        directionList.Add(new Vector3(-1, 0, 0));
        directionList.Add(new Vector3(-1, 0, 1));
        directionList.Add(new Vector3(0, 0, -1));
        directionList.Add(new Vector3(0, 0, 1));
        directionList.Add(new Vector3(1, 0, -1));
        directionList.Add(new Vector3(1, 0, 0));
        directionList.Add(new Vector3(1, 0, 1));
    }
}
