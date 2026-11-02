using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float yOffset = 2f;

    DamageTextSpawner spawner;
    Sequence seq;
    Camera cam;

    private void Awake() => cam = Camera.main;

    private void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }
    public void Show(int damage)
    {
        seq?.Kill();

        text.text = damage.ToString();
        text.alpha = 1f;
        transform.localScale = Vector3.zero;

        float startY = transform.position.y;

        seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack))
           .Append(transform.DOScale(1f, 0.1f))
           .Insert(0f, transform.DOMoveY(startY + 0.2f, 1.5f).SetEase(Ease.OutSine))
           .Insert(0.8f, text.DOFade(0f, 0.7f))
           .OnComplete(AnimationEnd);

        seq.SetLink(gameObject);
    }

    public void SetData(DamageTextSpawner damageTextSpawner,Transform trf, int damage)
    {
        spawner = damageTextSpawner;
        //위치 보정
        Vector3 pos = new Vector3(trf.position.x, trf.position.y + yOffset, trf.position.z);
        transform.position = pos;
        Show(damage);
    }
    private void AnimationEnd()
    {
        if (spawner == null) { return; }
        spawner.ReturnPrefabToPool(this);
        spawner = null;
    }
}
