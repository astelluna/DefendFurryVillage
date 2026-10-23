using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    //메인화면에서 금액을 표시해주는 UI 객체 
    [SerializeField] TextMeshProUGUI currencyText;
    CurrencyManager _currencyManager;
    private void Awake()
    {
        _currencyManager = CurrencyManager.Instance;
        if(_currencyManager == null)
        {
            MyLog.LogError("Currency매니저 못찾음");
            return;
        }
        _currencyManager.OnGameMoneyChange += UpdateUI;
        UpdateUI(_currencyManager.gameMoney);
    }
    private void OnDestroy()
    {
        if(_currencyManager != null)
        {
            _currencyManager.OnGameMoneyChange -= UpdateUI;
        }
        _currencyManager = null;
    }

    private void UpdateUI(long money)
    {
        currencyText.text = money.ToString();
    }
}
