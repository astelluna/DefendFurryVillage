using System;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    //그냥 일단은 로그인 패널을 띄우기 위한 스크립트로만 동작한다. 
    [SerializeField] GuestLogin guestLogin;
    [SerializeField] GoogleLogin googleLogin;
    public Action LoginCompleteRequest;

    [SerializeField] LoginFailPanel loginFailPanel;


    private void Awake()
    {
        guestLogin.GusetLoginRequest += GuestLoginProgress;
        googleLogin.GoogleLoginRequest += GoogleLoginProgress;
        loginFailPanel.CloseButtonRequest += ClosePanel;
    }
    private void OnDestroy()
    {
        guestLogin.GusetLoginRequest -= GuestLoginProgress;
        googleLogin.GoogleLoginRequest -= GoogleLoginProgress;
        loginFailPanel.CloseButtonRequest -= ClosePanel;
    }
    private void GoogleLoginProgress(bool isSuccess)
    {
        if (isSuccess)
        {
            LoginCompleteRequest?.Invoke();
            ClosePanel();
        }
        else
        {
            loginFailPanel.ShowPanel();
        }
    }
    private void GuestLoginProgress(bool isSuccess)
    {
        if(isSuccess)
        {
            LoginCompleteRequest?.Invoke();
            ClosePanel();
        }
        else
        {
            loginFailPanel.ShowPanel();
        }
    }

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    private void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }
}
