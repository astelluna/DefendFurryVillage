using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    //게임의 결과를 판단해서 패널을 띄운다. 
    [SerializeField] GameResultPanel gameResultPanel;

    [SerializeField] CastleWall castleWall; //성벽
    [SerializeField] LoadingPanelInGame loadingPanelInGame;

    private void Awake()
    {
        StageManager.Instance.StageClearRequest += ClearGame;
        castleWall.CastleDestroyRequest += DefeatGame;

    }
    private void OnDestroy()
    {
        StageManager.Instance.StageClearRequest -= ClearGame;
        castleWall.CastleDestroyRequest -= DefeatGame;
    }

    private async void ClearGame(StageDataSO stageDataSO)
    {
        ShowGameResultPanel(true, stageDataSO.RewardData);
        loadingPanelInGame.ShowLoadingPanel();
        //db에 보상 지급하고, 알아서 런타임 데이터도 동기화됨.
        await FirestoreManager.Instance.GiveReward(stageDataSO.RewardData);
        loadingPanelInGame.UnShowLoadingPanel();

    }
    private void DefeatGame()
    {
        ShowGameResultPanel(false);
    }
    private void ShowGameResultPanel(bool isClear, RewardDataSO rewardData = null)
    {
        Time.timeScale = 0f;
        gameResultPanel.ShowGameResultPanel(isClear, rewardData);
    }



}
