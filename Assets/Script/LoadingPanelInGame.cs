using System.Threading.Tasks;
using UnityEngine;

public class LoadingPanelInGame : MonoBehaviour
{
    //[SerializeField] StageManager 
    [SerializeField] StagePanel stagePanel;
    [SerializeField] StageEnemySpawnPosition stageEnemySpawnPosition;
    public void ShowLoadingPanel()
    {
        this.gameObject.SetActive(true);
    }
    public void UnShowLoadingPanel()
    {
        this.gameObject.SetActive(false);
    }

    private async void Start()
    {
        ShowLoadingPanel();
        StageManager.Instance.SetIngameData(stagePanel, stageEnemySpawnPosition);

        //게임 시작전 기다려야할것들 로딩
        try
        {
            Task stageInit = StageManager.Instance.InitForIngameScene();
            await CharacterManager.Instance.LoadSelectedCharacter();
            await CharacterManager.Instance.InitalizeCharacterTask;
            await CharacterManager.Instance.IngameCharacterLoadTask;
            await stageInit;
        }
        catch(System.Exception e)
        {
            MyLog.LogError(e);
            return;
        }
        
        //로딩패널 해제하고 게임 시작
        UnShowLoadingPanel();
        AugmentManager.Instance.SetAndShowAugmentUI(1);
        StageManager.Instance.WaveStart();

    }

}
