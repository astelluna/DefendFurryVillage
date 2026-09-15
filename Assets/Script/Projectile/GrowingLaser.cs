using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingLaser : Projectile
{
    [Header("길이/성장")]
    public float maxLength = 20f;
    public float growSpeed = 30f;
    public LayerMask hitLayers;      // 레이저가 막히는 대상 (벽 등)
    //public LayerMask targetLayers;   // 데미지를 줄 대상 (적 등)

    [Header("판정")]
    public float laserRadius = 0.7f;
    //public int damage = 10;
    public bool oneTimeDamagePerTarget = true; // 대상당 한 번만 데미지 줄지 여부

    [Header("비주얼")]
    public LineRenderer lineRenderer;

    private float currentLength = 0f;
    private float targetLength;
    private HashSet<Collider> alreadyHit = new HashSet<Collider>(); // 중복 방지용

    float maxLifeTime = 3f;
    float curLifeTime = 0f;

    void CheckHits()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            laserRadius,
            transform.forward,
            currentLength,
            TargetManager.Instance.EnemyLayer
        );

        foreach (var hit in hits)
        {
            if (oneTimeDamagePerTarget)
            {
                // 이미 맞은 대상이면 스킵
                if (alreadyHit.Contains(hit.collider)) continue;
                alreadyHit.Add(hit.collider);
                if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                {

                    ProcessHit(enemy);
                    //DamageInfo damageInfo = MakeDamageInfo(enemy);
                    //int calcDamage = DamageManager.Instance.CalcDamage(damageInfo);
                    //enemy.TakeDamage(calcDamage);
                    //Destroy(enemy.gameObject);
                }
                //if (hit.collider.TryGetComponent<Health>(out var health))
                //    health.TakeDamage(damage);
            }
            else
            {
                // 지속 데미지 (프레임마다 계속 깎임)
                //if (hit.collider.TryGetComponent<Health>(out var health))
                //  health.TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    public override void Shoot()
    {
        lineRenderer.useWorldSpace = false; // 명시적으로

        targetLength = maxLength;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero); // 로컬 원점

        Move();   
    }

    protected override void Move()
    {
        StartCoroutine(LaserMove());
    }
    IEnumerator LaserMove()
    {
        while(curLifeTime < maxLifeTime)
        {
            curLifeTime += Time.deltaTime;
            // 1. 길이 성장
            float previousLength = currentLength;
            if (currentLength < targetLength)
            {
                currentLength += growSpeed * Time.deltaTime;
                currentLength = Mathf.Min(currentLength, targetLength);
            }

            // 2. 비주얼 갱신
            // 로컬 좌표로 수정
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.forward * currentLength); // 로컬 forward
                                                                          // 3. 판정 (자라난 만큼만 검사)
            CheckHits();
            yield return null;
        }
        //전부 끝나면 바로 레이저 삭제
        DestroySelfSpecial();
    }

    protected override void Init()
    {
        
    }

    protected override void DestroySelfSpecial()
    {
        curLifeTime = 0;
        //Destroy(this.gameObject);
    }
}