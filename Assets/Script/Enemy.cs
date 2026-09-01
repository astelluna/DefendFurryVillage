using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    int damage;
    [SerializeField] float speed;
    //추가 빙결, 그런 효과들은 따로 처리
    //List<IBulletEffect> effectList = new List<IBulletEffect>();

    [SerializeField] Rigidbody rb;

    //public void SetEnemyInfo(int dmg, float spd, List<IBulletEffect> effects)
    //{
    //    damage = dmg;
    //    speed = spd;
    //    effectList = effects;
    //}

    private void Update()
    {
        //앞으로 이동. 물리로 되어야함. rb 필요
        //Test용
        //Vector3 vec = new Vector3(0, 0, -speed * Time.deltaTime);
        //rb.linearVelocity = vec;

    }
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * -speed * Time.fixedDeltaTime);
    }

    void OnHit()
    {
        //TODO -> 충돌 처리
    }
}

