using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    //이펙트 객체들을 생성하는 스포너
    public static EffectSpawner Instance { get; private set; }
    [SerializeField] int poolSize = 15;

    //이펙트 : 지연 생성으로 변경
    Dictionary<EffectScript, ObjPool<EffectScript>> effectPoolDict = new Dictionary<EffectScript, ObjPool<EffectScript>>();


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

    public EffectScript GetPrefabByPool(EffectScript effectScript)
    {
        //딕셔너리에 있는지 확인하고, 없으면 만든다.
        if(!effectPoolDict.ContainsKey(effectScript))
        {
            //새로 넣어주자.
            ObjPool<EffectScript> poolBuf  = new ObjPool<EffectScript>(effectScript, poolSize, gameObject.transform);
            effectPoolDict.Add(effectScript, poolBuf);
        }
        //이제 풀에서 하나 빼서 반환해준다.
        ObjPool<EffectScript> effectPool =  effectPoolDict[effectScript];
        EffectScript effectBuf =  effectPool.GetObject();
        effectBuf.SetPool(effectPool);
        return effectBuf;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    public void ReturnPrefabToPool(EffectScript effectScript)
    {
        //필요없음 -> 스크립트 안에서 파티클 이벤트 끝나면 알아서 반환
    }
}
