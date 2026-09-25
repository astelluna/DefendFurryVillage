using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatIcon : MonoBehaviour
{
    [SerializeField] Image statImage;
    [SerializeField] TextMeshProUGUI statText;

    [SerializeField] string basicText;

    public void SetText(string text)
    {
        //statImage.sprite = sprite; //이렇게 안하고 그냥 미리 넣어두자
        statText.text = basicText + text;
    }
}
