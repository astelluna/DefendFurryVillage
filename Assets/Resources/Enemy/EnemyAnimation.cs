using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    private Animator animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly string WallDetected = "WallDetected";
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void EnemyAttack()
    {
        animator.SetTrigger(AttackHash);
    }
    public void UpdateWallDetectState(bool isWallDetect)
    {
        animator.SetBool(WallDetected, isWallDetect);
    }
}
