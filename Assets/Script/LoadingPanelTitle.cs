using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanelTitle : MonoBehaviour
{
    //[SerializeField] Button testButton;
    Action<bool> completeAction;

    //패널
    [SerializeField] LoginPanel loginPanel;

    private void Awake()
    {
        Invoke(nameof(OnStartButtonClick), 3f);
        //testButton.onClick.AddListener(OnButtonClick);
        completeAction += Complete;
        loginPanel.LoginCompleteRequest += OnLoginComplete;
    }
    private void OnDestroy()
    {
        //testButton.onClick.RemoveAllListeners();
        completeAction -= Complete;
        loginPanel.LoginCompleteRequest -= OnLoginComplete;
    }

    private void OnStartButtonClick()
    {
        AddressableUpdateManager.Instance.StartUpdateCheck(completeAction);
    }


    private async void Complete(bool isSuccess)
    {
        MyLog.Log("어드레서블 다운로드  결과: " + isSuccess);
        if(!isSuccess)
        {
            MyLog.LogError("어드레서블 다운로드 실패");
            // TODO: 재시도 / 안내 UI
            return;
        }
        //bool isPreloadSuccess = await AddressablePreloadManager.Instance.StartPreLoad();
        bool isPreloadSuccess = await AddressableManager.Instance.StartPreload();

        if (!isPreloadSuccess)
        {
            MyLog.LogError("프리로드 실패");
            return;
        }

        bool autoLogin = await FirebaseAuthManager.Instance.TryAutoLoginAsync();
        if (autoLogin)
        {
            Debug.Log($"자동 로그인: {FirebaseAuthManager.Instance.User.UserId}");
            // 바로 로비/메인 씬으로 이동
            await LoginCompleteAsync();
        }
        else
        {
            //로그인 창이 뜨게 하자.
            loginPanel.ShowPanel();
            this.gameObject.SetActive(false);
        }

    }
    private async void OnLoginComplete()
    {
        gameObject.SetActive(true);
        await LoginCompleteAsync();
    }
    private async Task LoginCompleteAsync()
    {
        try
        {
            //로그인 성공 이후 필수 작업들을 진행해보자.
            await FirebaseAuthManager.Instance.AuthTcs.Task;
            string uid = FirebaseAuthManager.Instance.auth.CurrentUser.UserId;
            //아래부터 DB에서 최초로 정보 1회씩 불러와서 런타임에 설정.
            //0. 계정이 존재하는지 확인하고 없으면 만들어주기
            await FirestoreManager.Instance.CreateNewAccount(); //새로운 계정인지 체크해서 새 계정이면 데이터 만듬
                                                                //1. 가지고있는 캐릭터와 그 캐릭터의 장비 정보를 불러서 Charactermanager에 정보 설정
            await FirestoreManager.Instance.LoadAndSetHaveCharacter_Init();

            //2. 골드를 가져와서 셋팅
            long gold = await FirestoreManager.Instance.LoadGold();
            CurrencyManager.Instance.SetGameMoney(gold);

            //3. 인벤토리 - 아이템 동기화
            await FirestoreManager.Instance.InventoryData_Sync();
            //4. 스테이지 - 정보 동기화 
            await FirestoreManager.Instance.ProgressInit_Sync();
            //모두 끝나면 다음 씬으로 이동

            SceneManager.LoadScene(SceneController.Main);
        }
        catch (Exception e)
        {
            MyLog.LogError($"로그인 후 초기화 실패: {e}");
            // 실패 시 로그인 패널로 되돌리기 (무한 로딩 방지)
            loginPanel.ShowPanel();
            gameObject.SetActive(false);
        }

    }
}
