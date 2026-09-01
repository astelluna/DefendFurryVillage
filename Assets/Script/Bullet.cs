using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] Rigidbody rb;

    //TODO. 원래는 외부에서 속도를 받아야하긴하는데. 그냥 고정 속도로 해보자. 그게 나을듯
    float bulletSpeed = 5;

    bool isTriggered = false; //두개 이상 객체에 동시에 충돌해서 추가

    private void Awake()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }
    private void OnEnable()
    {
        isTriggered = false;
    }

    private void FixedUpdate()
    {
        //물리적으로 앞으로 이동한다.
        rb.MovePosition(transform.position + transform.forward * bulletSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0) return;
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            isTriggered = true;
            //TODO. 추후 오브젝트풀 반환시켜야하고, 바로 죽는게 아닌 데미지 처리하고 죽으면 반환해야함
            Destroy(other.gameObject);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void SetDirection(Vector3 dir)
    {
        this.transform.LookAt(dir);
    }
}
