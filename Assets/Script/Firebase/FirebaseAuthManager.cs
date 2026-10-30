using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    public FirebaseAuth auth; //인증 진행을 위한 객체
    public FirebaseUser User => auth?.CurrentUser; //인증되고나서, 인증된 유저 정보를 가지고있는다
    //public static DatabaseReference dbRef; //DB에 대한 정보를 여러 씬이서 다양하게 쓰려고
    public TaskCompletionSource<bool> AuthTcs = new TaskCompletionSource<bool>();
    public bool IsGuest
    {
        get
        {
            var u = User;
            if (u == null) return false;

            // 구글이 연결돼 있으면 게스트가 아님 (PC의 stale IsAnonymous 대비)
            foreach (var p in u.ProviderData)
                if (p.ProviderId == "google.com") return false;

            return u.IsAnonymous;
        }
    }
    protected override void OnAwake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result != Firebase.DependencyStatus.Available)
                {
                    AuthTcs.TrySetResult(false);
                    return;
                }
                auth = FirebaseAuth.DefaultInstance;
                AuthTcs.TrySetResult(true);
            });
    }
    
    /// <summary>
    /// 이전 로그인 기록이 있으면 true (자동 로그인 가능)
    /// </summary>
    public async Task<bool> TryAutoLoginAsync()
    {
        bool isReady = await AuthTcs.Task;
        if (!isReady) return false;

        FirebaseUser user = auth.CurrentUser;

        if (user == null) return false;

        try
        {
            // 서버에서 계정이 아직 유효한지 확인 (탈퇴/비활성화 대비)
            await user.ReloadAsync();
            return auth.CurrentUser != null;
        }
        catch (Exception e)
        {
            Debug.LogError($"자동 로그인 검증 실패: {e}");
            // 오프라인이면 캐시된 유저로 진행할지, 로그아웃할지는 정책에 따라 선택
            return false;
        }
    }
    public bool SignOut()
    {
        if (auth == null) return false;

        // 구글 로그인 세션도 정리 (모바일만)
        if (Application.isMobilePlatform)
        {
            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"구글 로그아웃 실패: {e.Message}");
                return false;
            }
        }

        auth.SignOut();
        return true;
    }
}
