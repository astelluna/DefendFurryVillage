using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour, IMainMenuPanel
{
    public void ShowPanel(bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (!isShow)
        {
            return;
        }
    }

}
