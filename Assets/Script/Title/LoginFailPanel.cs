using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginFailPanel : MonoBehaviour
{
    public event Action CloseButtonRequest;

    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    private void CloseButtonClick()
    {
        CloseButtonRequest?.Invoke();
        this.gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
}
