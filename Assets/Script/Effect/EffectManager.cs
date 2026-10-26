using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //전역 클래스로 만들기 
    public static EffectManager Instance { get; private set; }

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

    public void Playfbx(EffectScript originEffectPrefab, Transform trf)
    {
        //원본 이펙트 프리팹을 통해 오브젝트풀에서 생성되어있는 프리팹을 가져온다.
        EffectScript effectScript =  EffectSpawner.Instance.GetPrefabByPool(originEffectPrefab);
        effectScript.Play(trf.position, trf.rotation);
    }


}
