using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestAddressableUpdate : MonoBehaviour
{
    [SerializeField] Button testButton;
    Action<bool> completeAction;

    private void Awake()
    {
        testButton.onClick.AddListener(OnButtonClick);
        completeAction += Complete;
    }
    private void OnDestroy()
    {
        completeAction -= Complete;
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
            SceneManager.LoadScene(SceneController.Main);
        }

    }
}
