using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour, IMainMenuPanel
{
    [SerializeField] Button settingButton;
    [SerializeField] SettingPanel settingPanel;

    private void Awake()
    {
        settingButton.onClick.AddListener(OnSettingbuttonClick);
    }
    private void OnDestroy()
    {
        settingButton.onClick.RemoveAllListeners();
    }
    private void OnSettingbuttonClick()
    {
        settingPanel.ShowPanel();
    }

    public void ShowPanel(bool isShow)
    {
        this.gameObject.SetActive(isShow);
        if (!isShow)
        {
            return;
        }
    }

}
