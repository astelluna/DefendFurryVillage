using TMPro;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;


    public void UpdateLevelUpUI(int level, int exp)
    {
        levelText.text = level.ToString();
        experienceText.text = exp.ToString();
    }
}
