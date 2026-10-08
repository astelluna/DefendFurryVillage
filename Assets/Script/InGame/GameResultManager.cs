using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    //게임의 결과를 판단해서 패널을 띄운다. 
    [SerializeField] GameResultPanel gameResultPanel;

    [SerializeField] CastleWall castleWall; //성벽

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

    private void ClearGame()
    {
        ShowGameResultPanel(true);
    }
    private void DefeatGame()
    {
        ShowGameResultPanel(false);
    }
    private void ShowGameResultPanel(bool isClear)
    {
        Time.timeScale = 0f;
        gameResultPanel.ShowGameResultPanel(isClear);
    }



}
