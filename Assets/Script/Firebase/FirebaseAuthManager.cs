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
        //프로그램 구동과 동시에 비동기로 의존성을 체크한다((SDK, 네이티브 라이브러리, 설정)등 )
        //PC가 아닌 모바일 같은것에서 ContinueWith 코드 수행하면 안될수있음, 그래서 ContinueWithOnMainThread 사용
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available) //가능하다면
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance; //인증정보 기억
                //dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                MyLog.Log("FirebaseAuth 준비 완료");
                AuthTcs.SetResult(true);
            }
            else
            {
                MyLog.LogError(System.String.Format("뭔가 잘못됨" + task.Result));
                AuthTcs.SetResult(false);
            }
        });
    }
}
