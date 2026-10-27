using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUnEquipPanel : MonoBehaviour
{
    [SerializeField] Button unEquipButton;
    [SerializeField] Button closeButton;

    Action onConfirm;

    private void Awake()
    {
        unEquipButton.onClick.AddListener(OnUnEquipButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    private void OnDestroy()
    {
        onConfirm = null;
        unEquipButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnDisable()
    {
        OnCloseButtonClick();
    }
    private void OnUnEquipButtonClick()
    {
        onConfirm?.Invoke();
        onConfirm = null;
        OnCloseButtonClick();
    }
    private void OnCloseButtonClick()
    {
        onConfirm = null;
        this.gameObject.SetActive(false);
    }
    public void ClosePanel()
    {
        OnCloseButtonClick();
    }
    public void ShowPanel(Action action)
    {
        onConfirm = action;
        this.gameObject.SetActive(true);
    }
}


