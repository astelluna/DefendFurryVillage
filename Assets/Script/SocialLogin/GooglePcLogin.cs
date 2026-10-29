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

    public async Task<bool> StartSignIn()
    {
        // 1. 로컬 리스너 시작 (포트 5000)
        HttpListener listener = new HttpListener();
        try
        {
            listener.Prefixes.Add(redirectUri);
            listener.Start(); // 포트 사용 중이면 HttpListenerException
            string state = Guid.NewGuid().ToString("N");
            // 2. 구글 로그인 페이지 URL 생성 및 오픈
            string authUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                             $"client_id={clientId}&" +
                             $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                             "response_type=code&" +
                             "scope=openid%20email%20profile&" +
                             $"state={state}";
            Application.OpenURL(authUrl);
            MyLog.Log("브라우저에서 로그인을 완료해주세요...");

            // 3. 브라우저로부터 응답 대기 (비동기)
            var contextTask = listener.GetContextAsync();
            var finished = await Task.WhenAny(contextTask, Task.Delay(TimeSpan.FromSeconds(LoginTimeoutSec)));
            if (finished != contextTask)
            {
                MyLog.LogError("로그인 시간 초과");
                listener.Stop();
                return false;
            }

            HttpListenerContext context = contextTask.Result;
            var query = context.Request.QueryString;
            string code = query.Get("code");
            string error = query.Get("error");          // 유저가 '취소' 누르면 access_denied
            string returnedState = query.Get("state");
            bool ok = string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(code) && returnedState == state;



            // 브라우저에 완료 메시지 표시
            SendBrowserResponse(context, ok);

            if (!string.IsNullOrEmpty(error))
            {
                MyLog.LogError($"구글 로그인 거부/오류: {error}");
                return false;
            }
            if (returnedState != state)
            {
                MyLog.LogError("state 불일치 (위조된 요청 가능성)");
                return false;
            }
            if (string.IsNullOrEmpty(code))
            {
                MyLog.LogError("인증 코드가 없습니다");
                return false;
            }

            return await ExchangeCodeForFirebase(code);
        }
        catch (HttpListenerException e)
        {
            MyLog.LogError($"로컬 리스너 시작 실패 (포트 사용 중?): {e.Message}");
            return false;
        }
        finally
        {
            listener.Close(); //Stop + 해제
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
            MyLog.LogError($"브라우저 응답 실패: {e.Message}");  // 로그인 흐름 자체엔 영향 없음
        }
    }
    private async Task<bool> ExchangeCodeForFirebase(string code)
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
                while (!op.isDone) await Task.Yield();   // Unity 버전 상관없이 동작

                if (www.result != UnityWebRequest.Result.Success)
                {
                    // 구글이 주는 에러 본문(invalid_grant 등)도 같이 남기면 원인 찾기 쉬움
                    MyLog.LogError($"토큰 교환 실패: {www.error} / {www.downloadHandler.text}");
                    return false;
                }

                var tokenData = JsonUtility.FromJson<GoogleTokenResponse>(www.downloadHandler.text);
                if (tokenData == null || string.IsNullOrEmpty(tokenData.id_token))
                {
                    MyLog.LogError("id_token이 응답에 없습니다");
                    return false;
                }

                Credential credential = GoogleAuthProvider.GetCredential(tokenData.id_token, null);
                var auth = FirebaseAuth.DefaultInstance;

                FirebaseUser newUser = await auth.SignInWithCredentialAsync(credential);
                MyLog.Log("PC 로그인 성공! Firebase UID: " + newUser.UserId);
                return true;
            }
        }
        catch (Firebase.FirebaseException e)
        {
            var code2 = (AuthError)e.ErrorCode;
            MyLog.LogError($"Firebase 로그인 실패 ({code2}): {e.Message}");
            return false;
        }
        catch (Exception e)
        {
            MyLog.LogError($"ExchangeCodeForFirebase 예외: {e}");
            return false;
        }
    }
}
