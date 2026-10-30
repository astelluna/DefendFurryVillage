using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;
using UnityEngine.Networking;
using System.ComponentModel.Design.Serialization;

public class GooglePCLogin : MonoBehaviour
{
    //[SerializeField] Button _googlePcLoginButton;

    private string clientId = "841260331174-4l2m2d0d26kqg49lhngkoptj516vcg9q.apps.googleusercontent.com";
    private string clientSecret = "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";
    private string redirectUri = "http://localhost:5000/";

    private const int LoginTimeoutSec = 300;

    [Serializable]
    public class GoogleTokenResponse
    {
        public string access_token;
        public string id_token;
        public int expires_in;
    }

    // ───────── 외부에서 부르는 메서드 2개 ─────────

    // 일반 로그인 (기존과 똑같이 사용: bool 반환)
    public async Task<bool> StartSignIn()
    {
        LinkResult result = await RunFlow(false);
        return result == LinkResult.Success;
    }

    // 게스트 -> 구글 연동 (LinkResult 반환)
    public Task<LinkResult> StartLink()
    {
        return RunFlow(true);
    }

    // ───────── 공통 흐름 ─────────

    private async Task<LinkResult> RunFlow(bool link)
    {
        // 연동인데 게스트가 아니면 브라우저 열기 전에 바로 실패 처리
        if (link)
        {
            var cu = FirebaseAuth.DefaultInstance.CurrentUser;
            if (cu == null || !cu.IsAnonymous)
            {
                MyLog.LogError("게스트 상태가 아니라서 연동할 수 없습니다");
                return LinkResult.Fail;
            }
        }

        HttpListener listener = new HttpListener();
        try
        {
            listener.Prefixes.Add(redirectUri);
            listener.Start(); // 포트 사용 중이면 HttpListenerException

            string state = Guid.NewGuid().ToString("N");
            string authUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                             $"client_id={clientId}&" +
                             $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                             "response_type=code&" +
                             "scope=openid%20email%20profile&" +
                             $"state={state}";
            Application.OpenURL(authUrl);
            MyLog.Log("브라우저에서 로그인을 완료해주세요...");

            var contextTask = listener.GetContextAsync();
            var finished = await Task.WhenAny(contextTask, Task.Delay(TimeSpan.FromSeconds(LoginTimeoutSec)));
            if (finished != contextTask)
            {
                MyLog.LogError("로그인 시간 초과");
                return LinkResult.Fail;
            }

            HttpListenerContext context = contextTask.Result;
            var query = context.Request.QueryString;
            string code = query.Get("code");
            string error = query.Get("error");
            string returnedState = query.Get("state");
            bool ok = string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(code) && returnedState == state;

            SendBrowserResponse(context, ok);

            if (!string.IsNullOrEmpty(error))
            {
                MyLog.LogError($"구글 로그인 거부/오류: {error}");
                return LinkResult.Fail;
            }
            if (returnedState != state)
            {
                MyLog.LogError("state 불일치 (위조된 요청 가능성)");
                return LinkResult.Fail;
            }
            if (string.IsNullOrEmpty(code))
            {
                MyLog.LogError("인증 코드가 없습니다");
                return LinkResult.Fail;
            }

            return await ExchangeCodeForFirebase(code, link);
        }
        catch (HttpListenerException e)
        {
            MyLog.LogError($"로컬 리스너 시작 실패 (포트 사용 중?): {e.Message}");
            return LinkResult.Fail;
        }
        finally
        {
            listener.Close();
        }
    }

    private void SendBrowserResponse(HttpListenerContext context, bool success)
    {
        try
        {
            string msg = success
                ? "Login complete! You can close this window."
                : "Login failed. Please return to the game and try again.";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes($"<html><body>{msg}</body></html>");
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }
        catch (Exception e)
        {
            MyLog.LogError($"브라우저 응답 실패: {e.Message}");
        }
    }

    private async Task<LinkResult> ExchangeCodeForFirebase(string code, bool link)
    {
        try
        {
            WWWForm form = new WWWForm();
            form.AddField("code", code);
            form.AddField("client_id", clientId);
            form.AddField("client_secret", clientSecret);
            form.AddField("redirect_uri", redirectUri);
            form.AddField("grant_type", "authorization_code");

            using (UnityWebRequest www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form))
            {
                www.timeout = 20;
                var op = www.SendWebRequest();
                while (!op.isDone) await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    MyLog.LogError($"토큰 교환 실패: {www.error} / {www.downloadHandler.text}");
                    return LinkResult.Fail;
                }

                var tokenData = JsonUtility.FromJson<GoogleTokenResponse>(www.downloadHandler.text);
                if (tokenData == null || string.IsNullOrEmpty(tokenData.id_token))
                {
                    MyLog.LogError("id_token이 응답에 없습니다");
                    return LinkResult.Fail;
                }

                Credential credential = GoogleAuthProvider.GetCredential(tokenData.id_token, null);
                var auth = FirebaseAuth.DefaultInstance;

                if (link)
                {
                    // 게스트 계정에 구글 계정을 붙임 (UID 유지)
                    var currentUser = auth.CurrentUser;
                    if (currentUser == null || !currentUser.IsAnonymous)
                        return LinkResult.Fail;

                    AuthResult linkResult = await currentUser.LinkWithCredentialAsync(credential);
                    await linkResult.User.ReloadAsync();

                    MyLog.Log("PC 연동 성공! UID: " + linkResult.User.UserId);
                }
                else
                {
                    // 일반 로그인
                    FirebaseUser newUser = await auth.SignInWithCredentialAsync(credential);
                    MyLog.Log("PC 로그인 성공! Firebase UID: " + newUser.UserId);
                }



                return LinkResult.Success;
            }
        }
        catch (Exception e)
        {
            var inner = e is AggregateException ae ? ae.Flatten().InnerException : e;

            if (inner is Firebase.FirebaseException fe)
            {
                var errCode = (AuthError)fe.ErrorCode;
                MyLog.LogError($"Firebase 실패 ({errCode}): {fe.Message}");

                if (errCode == AuthError.CredentialAlreadyInUse ||
                    errCode == AuthError.EmailAlreadyInUse ||
                    errCode == AuthError.AccountExistsWithDifferentCredentials)
                {
                    return LinkResult.AlreadyInUse;
                }
                return LinkResult.Fail;
            }

            MyLog.LogError($"ExchangeCodeForFirebase 예외: {e}");
            return LinkResult.Fail;
        }
    }
}
