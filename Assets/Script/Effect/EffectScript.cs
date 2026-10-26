using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectScript : MonoBehaviour
{
    //풀을 등록하고, 이펙트가 끝났을떄 알아서 풀에 등록할수있다.
    private ParticleSystem _ps;
    private ObjPool<EffectScript> _pool; //커스텀 오브젝트풀

    private void Awake()
    {
        //외부에서 이펙트 없을때 Instantiate로 만드는데, 그거는 무조건 Awake 처리하므로 이 코드 무조건 실행됨
        _ps = GetComponent<ParticleSystem>();
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    public void SetPool(ObjPool<EffectScript> pool)
    {
        _pool = pool;
    }
    public void Play(Vector3 pos, Quaternion rotation)
    {
        transform.SetPositionAndRotation(pos, rotation);
        _ps.Clear(true);
        _ps.Play(true);
    }

    private void OnParticleSystemStopped()
    {
        //main.stopAction으로 인해 이펙트 끝나면 자동으로 이 메서드 실행됨
        _pool.ReturnObject(this);
        MyLog.Log("파티클 끝남");
    }
}
