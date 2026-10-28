using UnityEngine;
using Firebase;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine.UI;
using System;

public class GuestLogin : MonoBehaviour
{
    //FirebaseAuthManager의 작업이 끝났는지 대기하는것도 필요할듯
    FirebaseUser user;

    [SerializeField] Button _guestLoginButton;

    public event Action<bool> GusetLoginRequest;

    private void Awake()
    {
        _guestLoginButton.onClick.AddListener(GuestLoginButtonClick);
    }
    private void OnDestroy()
    {
        _guestLoginButton.onClick.RemoveAllListeners();
    }

    private async void GuestLoginButtonClick()
    {
        await SignInAnonymously();
    }

    private async Task SignInAnonymously()
    {
        try
        {
            bool isReady = await FirebaseAuthManager.Instance.AuthTcs.Task;
            if(!isReady)
            {
                MyLog.LogError("Firebase초기화 문제로 실패");
                GusetLoginRequest?.Invoke(false);
                return;
            }

            AuthResult result = await FirebaseAuthManager.Instance.auth.SignInAnonymouslyAsync();
            user = result.User;

            GusetLoginRequest?.Invoke(true);
            Debug.Log($"익명 로그인 성공: UID: {user.UserId}");
        }
        catch (System.Exception e)
        {
            GusetLoginRequest?.Invoke(false);
            Debug.LogError($"익명로그인 실패 {e}");
        }
    }
}