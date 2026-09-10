using UnityEngine;
[CreateAssetMenu(menuName = "Weapon/Augment/AugmentSpecialMagnet")]
public class AugmentSpecialMagnetSO : AugmentSpecialDataSO, IOnHit
{
    [SerializeField] float zOffset = 1.0f;
    [SerializeField] float radius = 1.0f;

    Collider[] colliderArray = new Collider[20];
    public override string GetDescription()
    {
        return $"주변의 적들을 끌어당깁니다 ";
    }
    public void OnHit(WeaponRuntimeData runtimeData, Enemy enemy)
    {
        //맞으면, 해당 범위의 주변 적들을 중앙으로 조금 끌고온다. -> 단, -z축으로

        Vector3 offsetPos = enemy.transform.position + (Vector3.forward * zOffset);
        int colliderCount = Physics.OverlapSphereNonAlloc(enemy.transform.position, radius, colliderArray, TargetManager.Instance.EnemyLayer);
        for(int i=0; i< colliderCount; i++)
        {
            if (colliderArray[i].TryGetComponent(out Enemy enemyBuf))
            {
                //자기자신을 해당 위치로 조금씩 이동시켜보자
                enemyBuf.StartEnemyMagnet(offsetPos, 3f);
            }
        }

    }
}
