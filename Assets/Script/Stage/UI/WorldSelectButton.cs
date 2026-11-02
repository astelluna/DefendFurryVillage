using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectButton : MonoBehaviour
{
    [SerializeField] Button worldSelectButton;
    [SerializeField] TextMeshProUGUI worldText;
    //[field: SerializeField] public string WorldName { get; private set; }
    public int WorldIndex { get; private set; } //월드번호는 1부터 시작
    public event Action<int> OnWorldSelectButton;

    private void Awake()
    {
        worldSelectButton.onClick.AddListener(WorldSelectButtonClick);
    }
    private void OnDestroy()
    {
        worldSelectButton.onClick.RemoveAllListeners();
    }

    private void WorldSelectButtonClick()
    {
        //버튼을 클릭하면 어떤 패널을 띄울거임. 
        OnWorldSelectButton?.Invoke(WorldIndex);
    }
    //public void SetWorldIndex(int idx)
    public void SetData(string worldName ,int idx)
    {
        worldText.text = worldName;
        WorldIndex = idx;
    }
    public void ActiveButton(bool isActive)
    {
        worldSelectButton.interactable = isActive;
    }

}
