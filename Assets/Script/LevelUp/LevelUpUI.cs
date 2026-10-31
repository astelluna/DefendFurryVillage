using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    //[SerializeField] TextMeshProUGUI experienceText;


    [SerializeField] Image fillImage;


    public void UpdateLevelUpUI(int level, float exp)
    {
        levelText.text = level.ToString();
        //experienceText.text = exp.ToString();

        fillImage.fillAmount = exp;
    }
}
