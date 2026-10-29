using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    //사용자가 PC로 로그인하는지, 모바일로 로그인하는지에 따라 다른 구조로 로그인 진행
    [SerializeField] Button googleLoginButton;
    [SerializeField] GoogleMobileLogin mobileLogin;
    [SerializeField] GooglePCLogin pcLogin;

    public event Action<bool> GoogleLoginRequest;

    private void Awake()
    {
        googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
    }
    private void OnDestroy()
    {
        googleLoginButton.onClick.RemoveAllListeners();
    }
    private async void OnGoogleLoginButtonClicked()
    {
        bool result = false;
        //로그인 진행
        if (Application.isMobilePlatform)
        {
            result = mobileLogin.GoogleSignInStart();

            MyLog.Log("Mobile");
        }
        else
        {
            result = await pcLogin.StartSignIn();
            MyLog.Log("PC");
        }
        GoogleLoginRequest?.Invoke(result);
    }
}
