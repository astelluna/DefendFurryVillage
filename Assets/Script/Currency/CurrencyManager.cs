using System;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public struct Currency
{
    
    [field: SerializeField] public eCurrencyType CurrencyType { get; private set; }
    [field: SerializeField] public long Amount { get; private set; }


    public void SetData(eCurrencyType type, long amt)
    {
        //이 메서드 이왕이면 사용하지말기. 애초에 CharacterSO에서 설정되서 나옴 
        CurrencyType = type;
        Amount = amt;
    }
}
public enum eCurrencyType
{
    gameMoney, paidMoney
}
public class CurrencyManager : Singleton<CurrencyManager>
{
    //재화를 관리하고 처리하는 매니저
    public long gameMoney { get; private set; }

    public event Action<long> OnGameMoneyChange;

    protected override void OnAwake()
    {
    }

    public void SetGameMoney(long amount)
    {
        //초기 DB에서 값을 읽어서 재화를 Set 해주는 메서드
        gameMoney = amount;
        OnGameMoneyChange?.Invoke(gameMoney);
    }
    public bool CanBuyProduct(long cost)
    {
        //게임머니로 살수있는지 체크
        if (gameMoney >= cost) { return true; }
        return false;
    }
}
