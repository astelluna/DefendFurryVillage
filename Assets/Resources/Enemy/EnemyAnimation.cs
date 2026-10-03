using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    [SerializeField] Animator animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly string WallDetected = "WallDetected";

    public void EnemyAttack()
    {
        animator.SetTrigger(AttackHash);
    }
    public void UpdateWallDetectState(bool isWallDetect)
    {
        animator.SetBool(WallDetected, isWallDetect);
    }
}
