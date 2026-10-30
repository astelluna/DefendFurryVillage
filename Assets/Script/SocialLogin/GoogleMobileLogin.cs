using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LinkResult { Success, Fail, AlreadyInUse }
public class GoogleMobileLogin : MonoBehaviour
{
    private FirebaseUser user;
    string webId = "841260331174-v0lsd4ujd9qqg6a87ed9k8a8ct15la0f.apps.googleusercontent.com";


    public async Task<bool> GoogleSignInAsync()
    {
        try
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };

            GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance.SignIn();

            Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);
            FirebaseUser fbUser = await FirebaseAuthManager.Instance.auth.SignInWithCredentialAsync(credential);

            Debug.Log($"로그인 성공: {fbUser.UserId} / {fbUser.DisplayName}");

            user = FirebaseAuthManager.Instance.auth.CurrentUser;

            Debug.Log($"UserName: {user.DisplayName}");
            Debug.Log($"UserEmail: {user.Email}");

            Debug.Log(user.UserId);
            Debug.Log(user.DisplayName);
            Debug.Log(SystemInfo.deviceModel);   // 기기 모델 samsung SM-SxxxN
            Debug.Log(SystemInfo.deviceName);    // 기기 이름 (길동의 S23 Ultra)
            Debug.Log(SystemInfo.deviceType);    // Deskto/ Console / Handheld :모바일 
            Debug.Log(SystemInfo.operatingSystem); // OS 정보 (Android OS 16 / API-36 (BP2A.250605.031.A3/S928NxxxxxZA1))
            return true;
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                if (e is GoogleSignIn.SignInException se)
                    Debug.LogError($"GoogleSignIn 실패 Status: {se.Status} / {se.Message}");
                else
                    Debug.LogError($"로그인 예외: {e}");
            }
            return false;
        }
        catch (GoogleSignIn.SignInException se)
        {
            Debug.LogError($"GoogleSignIn 실패 Status: {se.Status}");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"로그인 예외: {e}");
            return false;
        }
    }
    public async Task<LinkResult> LinkGoogleAsync()
    {
        //비회원 -> 구글 연동을 시도할때 이렇게 처리됨
        try
        {
            var currentUser = FirebaseAuthManager.Instance.User;
            if (currentUser == null || !currentUser.IsAnonymous)
                return LinkResult.Fail; // 게스트가 아니면 연동할 필요 없음

            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };

            GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance.SignIn();
            Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);

            await currentUser.LinkWithCredentialAsync(credential);

            // 연동 후 프로필(이메일, 이름) 갱신
            await FirebaseAuthManager.Instance.User.ReloadAsync();

            Debug.Log($"연동 성공: {FirebaseAuthManager.Instance.User.UserId}");
            return LinkResult.Success;
        }
        catch (Exception e)
        {
            var inner = e is AggregateException ae ? ae.Flatten().InnerException : e;

            if (inner is GoogleSignIn.SignInException se)
            {
                Debug.LogWarning($"구글 로그인 취소/실패: {se.Status}");
                return LinkResult.Fail;
            }

            if (inner is FirebaseException fe)
            {
                var code = (AuthError)fe.ErrorCode;
                if (code == AuthError.CredentialAlreadyInUse ||
                    code == AuthError.EmailAlreadyInUse ||
                    code == AuthError.AccountExistsWithDifferentCredentials)
                {
                    GoogleSignIn.DefaultInstance.SignOut(); // 선택 사항: 세션 정리
                    return LinkResult.AlreadyInUse;
                }
            }

            Debug.LogError($"구글 연동 실패: {e}");
            return LinkResult.Fail;
        }
    }
}