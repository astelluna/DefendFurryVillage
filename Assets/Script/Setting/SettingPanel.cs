using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    //계정 관련
    [SerializeField] Button logoutButton;
    [SerializeField] Button linkAccountButton;
    [SerializeField] Button creditButton;
    [SerializeField] Button closeButton;
    [SerializeField] LogoutPanel logoutPanel;
    [SerializeField] LinkSocialAccountPanel linkSocialAccountPanel;
    [SerializeField] CreditPanel creditPanel;
    private void Awake()
    {
        logoutButton.onClick.AddListener(ShowLogoutPanel);
        linkAccountButton.onClick.AddListener(ShowLinkPanel);
        creditButton.onClick.AddListener(ShowCreditPanel);
        closeButton.onClick.AddListener(ClosPanel);
    }
    private void OnEnable()
    {
        Refresh();
    }
    private void OnDisable()
    {
        ClosPanel();
    }
    private void OnDestroy()
    {
        logoutButton.onClick.RemoveAllListeners();
        linkAccountButton.onClick.RemoveAllListeners();
        creditButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private void ShowLogoutPanel()
    {
        logoutPanel.ShowPanel();
    }
    private void ShowLinkPanel()
    {
        linkSocialAccountPanel.ShowPanel();
    }
    private void ShowCreditPanel()
    {
        creditPanel.ShowPanel();
    }
    private void ClosPanel()
    {
        this.gameObject.SetActive(false);
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }

    // 게스트일 때만 연동 버튼 표시
    private void Refresh()
    {
        //게스트가 아니면 계정연동 버튼은 활성화 안함
        var User = FirebaseAuthManager.Instance.User;
        Debug.Log($"UserId : {User?.UserId}");
        Debug.Log($"IsAnonymous : {User?.IsAnonymous}");
        Debug.Log($"Email : {User?.Email}");
        Debug.Log($"ProviderId : {User?.ProviderId}");

        if (User != null)
        {
            foreach (var provider in User.ProviderData)
            {
                Debug.Log($"Provider : {provider.ProviderId}");
            }
        }
        Debug.Log($"ttta : {FirebaseAuthManager.Instance.IsGuest}");
        linkAccountButton.gameObject.SetActive(FirebaseAuthManager.Instance.IsGuest);
    }
}
