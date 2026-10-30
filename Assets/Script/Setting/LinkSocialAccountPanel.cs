using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkSocialAccountPanel : MonoBehaviour
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button closeButton;

    [SerializeField] GoogleMobileLogin mobileLogin;
    [SerializeField] GooglePCLogin pcLogin;
    [SerializeField] SettingInfoPanel settingInfoPanel;

    private void Awake()
    {
        confirmButton.onClick.AddListener(LinkAccount);
        closeButton.onClick.AddListener(ClosePanel);
    }
    private void OnDestroy()
    {
        confirmButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private async void LinkAccount()
    {
        // 진행 중 중복 클릭 방지
        confirmButton.interactable = false;
        closeButton.interactable = false;

        LinkResult result;
        try
        {
            if (Application.isMobilePlatform)
            {
                result = await mobileLogin.LinkGoogleAsync();
                Debug.Log("Mobile");
            }
            else
            {
                result = await pcLogin.StartLink();
                Debug.Log("PC");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"연동 예외: {e}");
            result = LinkResult.Fail;
        }
        if (this == null) return;

        confirmButton.interactable = true;
        closeButton.interactable = true;
        switch (result)
        {
            case LinkResult.Success:
                // 연동 성공 → 안내 후 종료
                settingInfoPanel.ShowPanel("계정이 연동되었습니다. 다시 로그인해주세요.", Quit);
                break;

            case LinkResult.AlreadyInUse:
                settingInfoPanel.ShowPanel("이미 다른 계정에 사용 중인 구글 계정입니다.", null);
                break;

            case LinkResult.Fail:
            default:
                settingInfoPanel.ShowPanel("연동에 실패했습니다. 다시 시도해주세요.", null);
                break;
        }
    }

    private void Quit()
    {
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
