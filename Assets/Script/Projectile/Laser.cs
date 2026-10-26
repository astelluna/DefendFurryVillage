using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile
{
    [Header("길이/성장")]
    public float maxLength = 20f;
    public float growSpeed = 60f;
    public LayerMask wallLayer; //벽

    [Header("판정")]
    public float laserRadius = 0.7f;

    [Header("비주얼")]
    public Transform beam;            // 스케일할 빔 메시 (자식)
    public Renderer beamRenderer;
    //public Transform impactFx;
    public float beamWidth = 0.6f;
    public float pulseSpeed = 25f;
    public float pulseAmount = 0.15f;
    public float scrollSpeed = 5f;
    public float tilingPerMeter = 0.5f;

    [Header("수명")]
    public float growTime = 0.1f;
    public float holdTime = 1.5f;
    public float fadeTime = 0.3f;

    float currentLength;
    HashSet<Collider> alreadyHit = new HashSet<Collider>();
    Material mat;

    public override void Shoot()
    {
        mat = beamRenderer.material;
        Move();
    }

    protected override void Move() => StartCoroutine(LaserRoutine());

    IEnumerator LaserRoutine()
    {
        float t = 0f;
        float total = growTime + holdTime + fadeTime;

        while (t < total)
        {
            t += Time.deltaTime;

            // 벽에 막히는 길이
            float limit = maxLength;
            if (Physics.Raycast(transform.position, transform.forward,
                out RaycastHit wall, maxLength, wallLayer))
                limit = wall.distance;

            //z축 길이 (앞으로 이동) 
            currentLength = Mathf.MoveTowards(currentLength, limit, growSpeed * Time.deltaTime);

            // 두께 애니메이션
            float w;
            if (t < growTime) w = t / growTime;
            else if (t < growTime + holdTime) w = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            else w = 1f - (t - growTime - holdTime) / fadeTime;
            w = Mathf.Max(0f, w) * beamWidth;

            // 핵심: z 스케일 + 위치 보정 (뒤쪽 끝이 발사 지점에 고정되도록)
            beam.localScale = new Vector3(w, w, currentLength);
            beam.localPosition = new Vector3(0, 0, currentLength * 0.5f); //중심을 이동시켜서

            // 텍스처 타일링/스크롤
            mat.mainTextureScale = new Vector2(currentLength * tilingPerMeter, 1);
            mat.mainTextureOffset = new Vector2(-Time.time * scrollSpeed, 0);

            // 임팩트는 위치만
            //if (impactFx) impactFx.localPosition = Vector3.forward * currentLength;

            if (t < growTime + holdTime) CheckHits();
            yield return null;
        }

        DestroySelf();
    }

    void CheckHits()
    {
        var hits = Physics.SphereCastAll(transform.position, laserRadius,
            transform.forward, currentLength, TargetManager.Instance.EnemyLayer);

        foreach (var hit in hits)
        {
            if (!alreadyHit.Add(hit.collider)) continue;
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                ProcessHit(enemy);
        }
    }

    protected override void Init() { }

    protected override void DestroySelfSpecial()
    {
        alreadyHit.Clear();
        currentLength = 0f;
    }
}
