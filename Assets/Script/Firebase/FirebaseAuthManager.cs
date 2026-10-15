using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    public FirebaseAuth auth; //인증 진행을 위한 객체
    public static FirebaseUser user; //인증되고나서, 인증된 유저 정보를 가지고있는다
    //public static DatabaseReference dbRef; //DB에 대한 정보를 여러 씬이서 다양하게 쓰려고
    public TaskCompletionSource<bool> AuthTcs = new TaskCompletionSource<bool>();
    protected override void OnAwake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(async task =>
            {
                if (task.Result != Firebase.DependencyStatus.Available)
                {
                    AuthTcs.TrySetResult(false);
                    return;
                }

                auth = FirebaseAuth.DefaultInstance;

                //테스트할때 이거 켜놓기
                //SignOut_Test();

                AuthTcs.TrySetResult(true);
            });
    }
    private void SignOut_Test()
    {
        Debug.Log($"로그아웃 전 UID: {auth.CurrentUser?.UserId ?? "null"}");
        auth.SignOut();
        Debug.Log($"로그아웃 후 UID: {auth.CurrentUser?.UserId ?? "null"}");
    }
}
