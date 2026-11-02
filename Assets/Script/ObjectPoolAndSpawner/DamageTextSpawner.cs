using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    //이펙트 객체들을 생성하는 스포너
    public static DamageTextSpawner Instance { get; private set; }

    [SerializeField] DamageText prefab;
    [SerializeField] int poolSize = 25;

    ObjPool<DamageText> pool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pool = new ObjPool<DamageText>(prefab, poolSize, gameObject.transform);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public DamageText GetPrefabByPool(Transform damagePos, int dmg)
    {
        DamageText prefabBuf = pool.GetObject();
        prefabBuf.SetData(this,damagePos, dmg);
        return prefabBuf;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    public void ReturnPrefabToPool(DamageText prefabObject)
    {
        prefabObject.transform.SetParent(this.transform, false);
        pool.ReturnObject(prefabObject);
    }
}
