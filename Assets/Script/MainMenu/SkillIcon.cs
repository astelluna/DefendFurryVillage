using TMPro;
using UnityEngine;
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
}
