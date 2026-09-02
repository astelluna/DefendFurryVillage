using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    float radius;
    [SerializeField] float duration = 0.3f;
    [SerializeField] float arcHeight = 3f;

    Vector3 startPosition;
    Vector3 targetPosition;

    //public void Shoot(Vector3 target, float explodeRadius)
    public void Shoot(Vector3 target, float explodeRadius)
    {
        startPosition = transform.position;
        targetPosition = target;
        radius = explodeRadius;

        StartCoroutine(Move());
    }


    IEnumerator Move()
    {

        //해당 방향을 바라보도록 하자.
        transform.LookAt(targetPosition);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            Vector3 position =
                Vector3.Lerp(startPosition, targetPosition, t);

            transform.position = position;

            yield return null;
        }

        Explode();
    }



    void Explode()
    {
        var colliders = Physics.OverlapSphere(transform.position, radius, TargetManager.Instance.EnemyLayer);
        foreach(var item in colliders)
        {
            //TODO. 추후에는 그냥 enemy에 데미지 주면 됨.
            //일단은 그냥 없앤다.
            if (item.TryGetComponent(out Enemy enemy))
            {
                Destroy(enemy.gameObject);
            }
        }
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);
    }


}
