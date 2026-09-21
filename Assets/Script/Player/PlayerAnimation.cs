using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    //애니메이터의 파라미터 -> 트리거 항목을 켜준다.
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int AttackSpecialHash = Animator.StringToHash("SpecialAttack");

    void Start()
    {
        animator = GetComponent<Animator>();
        //Invoke(nameof(PlayAttackBasic), 3f);
    }

    public void PlayAttackBasic()
    {
        animator.SetTrigger(AttackHash);
    }
    public void PlayAttackSpecial()
    {
        animator.SetTrigger(AttackSpecialHash);
    }
}
