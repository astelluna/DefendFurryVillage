using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestAddressableUpdate : MonoBehaviour
{
    [SerializeField] Button testButton;
    Action<bool> completeAction;

    //패널
    [SerializeField] LoginPanel loginPanel;

    private void Awake()
    {
        testButton.onClick.AddListener(OnButtonClick);
        completeAction += Complete;
        loginPanel.LoginCompleteRequest += LoginComplete;
    }
    private void OnDestroy()
    {
        testButton.onClick.RemoveAllListeners();
        completeAction -= Complete;
        loginPanel.LoginCompleteRequest -= LoginComplete;
    }

    private void OnButtonClick()
    {
        AddressableUpdateManager.Instance.StartUpdateCheck(completeAction);
    }
    private void Complete(bool isSuccess)
    {
        MyLog.Log("어드레서블 다운로드  결과: " + isSuccess);

        if(isSuccess)
        {
            //로그인 창이 뜨게 하자.
            loginPanel.ShowPanel();


            //SceneManager.LoadScene(SceneController.Main); -> 보류;
        }

    }
    private async void LoginComplete()
    {
        //로그인 성공 이후 필수 작업들을 진행해보자.
        await FirebaseAuthManager.Instance.AuthTcs.Task;
        string uid = FirebaseAuthManager.Instance.auth.CurrentUser.UserId;
        //아래부터 DB에서 최초로 정보 1회씩 불러와서 런타임에 설정.
        //1. 가지고있는 캐릭터와 그 캐릭터의 장비 정보를 불러서 Charactermanager에 정보 설정
        await FirestoreManager.Instance.LoadAndSetHaveCharacter_Init();

        //2. 골드를 가져와서 셋팅
        long gold = await FirestoreManager.Instance.LoadGold();
        CurrencyManager.Instance.SetGameMoney(gold);

        //3. 인벤토리 - 아이템 동기화
        await FirestoreManager.Instance.InventoryData_Sync();
        //모두 끝나면 다음 씬으로 이동


        SceneManager.LoadScene(SceneController.Main);
    }
}
