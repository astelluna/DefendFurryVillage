using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingInfoPanel : MonoBehaviour
{
    //설정창에서 정보 혹은 에러 발생했을때 정보 표시하는 패널 
    [SerializeField] Button okButton;
    [SerializeField] TextMeshProUGUI infoText;

    Action action;

    private void Awake()
    {
        okButton.onClick.AddListener(OnOkButtonClicked);
    }
    private void OnDestroy()
    {
        okButton.onClick.RemoveAllListeners();
    }
    public void ShowPanel(string msg, Action act)
    {
        infoText.text = msg;
        action = act;
        this.gameObject.SetActive(true);
    }

    public void OnOkButtonClicked()
    {
        if(action != null)
        {
            action();
            return;
        }
        
        this.gameObject.SetActive(false);
    }
}
