using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image progressBar;


    public void SetImageRatio(float ratio)
    {
        progressBar.fillAmount = ratio;
    }
}
