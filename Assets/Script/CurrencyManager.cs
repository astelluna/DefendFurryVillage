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

    protected override void OnAwake()
    {
    }

    public void SetGameMoney(long amount)
    {
        //초기 DB에서 값을 읽어서 재화를 Set 해주는 메서드
        gameMoney = amount;
    }
    public async Task<bool> TrySpendMoney(int cost)
    {
        //돈을 써본다. 실제로 돈이 있다면 재화를 감소시키고 진행한다.
        long subValue = gameMoney - cost;
        if(subValue < 0)
        {
            //돈을 쓰지 못함.
            return false;
        }
        //쓸수있다면? 진행. FirestorManager 내에서 성공하면 알아서 동기화까지 진행함
        bool isSuccess =  await FirestoreManager.Instance.TrySpendGold(cost);
        return isSuccess;
    }
    public bool CanBuyProduct(long cost)
    {
        //게임머니로 살수있는지 체크
        if (gameMoney >= cost) { return true; }
        return false;
    }
}
