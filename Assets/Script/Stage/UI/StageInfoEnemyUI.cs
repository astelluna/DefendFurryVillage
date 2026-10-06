using UnityEngine;
using UnityEngine.UI;

public class StageInfoEnemyUI : MonoBehaviour
{
    [SerializeField] Image enemyImage;

    public void SetImage(Sprite sprite)
    {
        enemyImage.sprite = sprite;
        this.gameObject.SetActive(true);
    }
}
