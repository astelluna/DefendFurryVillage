using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogoutPanel : MonoBehaviour
{
    [SerializeField] Button logOutButton;
    [SerializeField] Button closeButton;

    [SerializeField] SettingInfoPanel infoPanel;

    private void Awake()
    {
        logOutButton.onClick.AddListener(LogOut);
        closeButton.onClick.AddListener(ClosePanel);
    }
    private void OnDestroy()
    {
        logOutButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private void LogOut()
    {
        bool isSuccess = FirebaseAuthManager.Instance.SignOut();
        if(!isSuccess)
        {
            string msg = "로그인 실패";
            infoPanel.ShowPanel(msg, null);
            return;
        }
        //게임 종료시키자
        Application.Quit();
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }
}
