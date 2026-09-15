using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : Projectile
{
    //[SerializeField] float basicRadius = 3f; //기본 폭발 범위
    [SerializeField] float duration = 0.3f;
    [SerializeField] float arcHeight = 3f;

    Vector3 startPosition;


    IEnumerator MoveExecute()
    {

        //해당 방향을 바라보도록 하자.
        transform.LookAt(targetPos);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            Vector3 position =
                Vector3.Lerp(startPosition, targetPos, t);

            transform.position = position;

            yield return null;
        }

        Explode();
    }



    void Explode()
    {
        //기본범위 + 스케일로 폭발범위를 조절하도록 하자
        var colliders = Physics.OverlapSphere(transform.position, explodeRadius, TargetManager.Instance.EnemyLayer);
        foreach(var item in colliders)
        {
            //TODO. 추후에는 그냥 enemy에 데미지 주면 됨.
            //일단은 그냥 없앤다.
            if (item.TryGetComponent(out Enemy enemy))
            {
                ProcessHit(enemy);
                //DamageInfo damageInfo = MakeDamageInfo(enemy);
                //int calcDamage = DamageManager.Instance.CalcDamage(damageInfo);
                //enemy.TakeDamage(calcDamage);
                //Destroy(enemy.gameObject);
            }
        }
        DestroySelf();
    }

    public override void Shoot()
    {
        startPosition = transform.position;
        Move();
    }

    protected override void Move()
    {
        StartCoroutine(MoveExecute());
    }

    protected override void Init()
    {
        
    }

    protected override void DestroySelfSpecial()
    {
        //Destroy(this.gameObject);
    }
}
