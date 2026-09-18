using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    //투사체에 대해서 여러 투사체들을 등록을 해두자.

    public static ProjectileSpawner Instance { get; private set; }

    [SerializeField] int poolSize = 10;

    Dictionary<Projectile, ObjPool<Projectile>> projectileDict = new Dictionary<Projectile, ObjPool<Projectile>>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void MakeProjectile(Projectile projectile)
    {
        if(projectileDict.ContainsKey(projectile))
        {
            return;
        }
        ObjPool<Projectile> projectilePool = new ObjPool<Projectile>(projectile, poolSize, gameObject.transform);
        projectileDict.Add(projectile, projectilePool);
    }
    public Projectile GetPrefabByPool(Transform trf, Projectile projectile)
    {
        if (!projectileDict.ContainsKey(projectile)) { return null; }

        ObjPool<Projectile> projectilePool = projectileDict[projectile];

        if(projectilePool == null)
        {
            MyLog.LogError("풀이 에러");
        }


        Projectile projectileBuf = projectilePool.GetObject();
        projectileBuf.transform.position = trf.position;
        projectileBuf.transform.rotation = trf.rotation;
        projectileBuf.SetObjectPool(projectilePool);
        projectileBuf.OnProjectileDestroy += ReturnPrefabToPool;
        //enemyBuf.OnEnemyDead += ReturnPrefabToPool;

        return projectileBuf;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    private void ReturnPrefabToPool(Projectile projectile)
    {
        //현재는 enemy에서 직접 반환해서 안씀
        //추후에 필요할때 넣자
        projectile.OnProjectileDestroy -= ReturnPrefabToPool;
        

        //if (!enemyDict.ContainsKey(enemy)) { return null; } //주의. 이렇게 딕셔너리로 검색하면 안된다 -> enemy는 원본 프리팹이 아니라 꺼낸 프리팹임

        //경험치를 주웠을때 
        // _getExpPoint?.Invoke(10);
    }



}
