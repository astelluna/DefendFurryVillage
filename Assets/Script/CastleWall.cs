using UnityEngine;

public class CastleWall : MonoBehaviour
{
    int hp = 1500;
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        //Debug.Log($"HP {hp}");
    }

}
