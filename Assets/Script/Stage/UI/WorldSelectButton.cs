using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectButton : MonoBehaviour
{
    [SerializeField] Button worldSelectButton;
    [SerializeField] TextMeshProUGUI worldText;
    [field: SerializeField] public string WorldName { get; private set; }
    [field: Header("월드번호는 1부터 시작")]
    [field:SerializeField] public int WorldIndex { get; private set; }
    public event Action<int> OnWorldSelectButton;

    private void Awake()
    {
        worldSelectButton.onClick.AddListener(WorldSelectButtonClick);
        worldText.text = WorldName;
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

}
