using System;
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
    private void LoginComplete()
    {
        SceneManager.LoadScene(SceneController.Main);
    }
}
