using TMPro;
using UnityEngine;

public class CastelWallUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wallHpText;

    public void UpdateWallHp(int amount)
    {
        wallHpText.text = amount.ToString();
    }
}
