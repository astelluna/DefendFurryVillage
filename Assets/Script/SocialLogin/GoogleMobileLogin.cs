using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMobileLogin : MonoBehaviour
{
    private FirebaseUser user;

    public bool GoogleSignInStart()
    {
        try
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration()
            {
                WebClientId = "841260331174-b3vbcnb40t07vi7r53qsoq04jfgnaiis.apps.googleusercontent.com",
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true,
            };

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    MyLog.LogError($"SignIn Error: {task.Exception}");
                }
                else if (task.IsCanceled)
                {
                    MyLog.LogError($"SignIn Canceled: ");
                }
                else
                {
                    OnGoogleAuthenticatedFinished(task);
                }
            });
        }
        catch (Exception e)
        {
            MyLog.LogError($"GoogleSignInClick Exception: {e.Message}");
            return false;
        }
        return true;
    }

    private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            MyLog.LogError("Faulted");
        }
        else if (task.IsCanceled)
        {
            MyLog.LogError("Cancelled");
        }
        else
        {
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            FirebaseAuthManager.Instance.auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(continuation =>
            {
                if (continuation.IsCanceled) { return; }

                if (continuation.IsFaulted)
                {
                    MyLog.LogError($"SignInWithCredentialAsync encountered an error: {continuation.Exception}");
                    return;
                }

                user = FirebaseAuthManager.Instance.auth.CurrentUser;

                MyLog.Log($"UserName: {user.DisplayName}");
                MyLog.Log($"UserEmail: {user.Email}");

                MyLog.Log(user.UserId);
                MyLog.Log(user.DisplayName);
                MyLog.Log(SystemInfo.deviceModel);   // 기기 모델 samsung SM-SxxxN
                MyLog.Log(SystemInfo.deviceName);    // 기기 이름 (길동의 S23 Ultra)
                MyLog.Log(SystemInfo.deviceType);    // Deskto/ Console / Handheld :모바일 
                MyLog.Log(SystemInfo.operatingSystem); // OS 정보 (Android OS 16 / API-36 (BP2A.250605.031.A3/S928NxxxxxZA1))



            });
        }
    }
}