using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] Image skillSprite;
    [SerializeField] TextMeshProUGUI skillDescText;

    public void SetIconInfo(Sprite sprite, string desc)
    {
        skillSprite.sprite = sprite;
        skillDescText.text = desc;
    }
    public void Clear()
    {
        skillSprite.sprite = null;
        skillDescText.text = "";
    }
}
