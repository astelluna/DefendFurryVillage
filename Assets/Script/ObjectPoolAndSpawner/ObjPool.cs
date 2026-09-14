using System.Collections.Generic;
using UnityEngine;

public class ObjPool<T> where T : MonoBehaviour
{
    //몬스터, 적, 총알, 파티클 등
    Queue<T> pool;
    private T prefab;
    Transform poolParent;   //하위객체에 넣어서 관리할때 사용
    int aliveCount; //풀에 활성화되어있는 객체 숫자
    public ObjPool(T prefab, int initSize, Transform parentPos = null)
    {
        this.prefab = prefab;
        pool = new Queue<T>();
        this.poolParent = parentPos;
        aliveCount = 0;

        for (int i = 0; i < initSize; i++)
        {
            //ObjPool은 MonoBehavior를 상속받지 않기때문에 Object.을 붙여야함, Where은 제약조건이지 상속을받지는않았다
            T instance;
            if (poolParent == null)
            {
                instance = Object.Instantiate(prefab, poolParent);
            }
            else
            {
                instance = Object.Instantiate(prefab, poolParent);
            }
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public T GetObject()
    {
        aliveCount++;
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        //풀이 모두 사용중이라면 풀을 늘림
        T newObj;
        if (poolParent == null)
        {
            newObj = Object.Instantiate(prefab);
        }
        else
        {
            newObj = Object.Instantiate(prefab, poolParent);
        }
        return newObj;
    }

    public void ReturnObject(T obj)
    {
        aliveCount--;
        pool.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
    public int AlivePoolCount()
    {
        return aliveCount;
    }
}