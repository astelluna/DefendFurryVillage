using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    //싱글톤 구조를 가져가지만 다음씬에서 제거되어야 할때 사용 *씬별로 존재하는 UI매니저 등)
    protected bool isDestroyOnLoad = true;
    //MVP에서 싱글톤 매니저의 이벤트를 해제(Dispose)해야되는데 이때 Null체크가 필요해서 추가
    //isHaveInstance를 안쓰고 if(Manager.Instance != null) 이렇게 하면 인스턴스에 접근해서 새로 생겨버림
    //public static bool isHaveInstance => _instance != null;

    //Instance 프로퍼티와 Awake에서 둘다 null체크를 하는 이유:
    //외부에서 Instance로 호출하는경우 생성되고
    //이미 씬에서 생성되어있을경우 프로퍼티로 호출되지는않고 Awake에서 최초 호출되므로

    //외부 호출용 프로퍼티, 해당 타입의 싱글톤이 없으면 찾아보고 그래도 없으면 새로 생성 후 결정
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>(); //찾아봄

                //if (_instance == null)
                //{
                //    GameObject singletonObj = new GameObject();
                //    _instance = singletonObj.AddComponent<T>();
                //    singletonObj.name = typeof(T).ToString();
                //}
            }
            return _instance;
        }
    }
    protected virtual void Awake() //중복체크 및 연결기능 구현 (재정의 가능)
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return; //이게 없으면 자식의 Awake가 끝까지 실행됨
        }
        else
        {
            _instance = this as T;
            if (isDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        OnAwake();
    }
    protected abstract void OnAwake();
}