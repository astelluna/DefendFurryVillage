using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI countText;
    //RewardItem rewardItem;
    
    public void SetData(RewardItem item)
    {
        //rewardItem = item;
        itemImage.sprite = item.itemDataSO.SpriteImage;
        countText.text = item.Count.ToString();
    }
}
