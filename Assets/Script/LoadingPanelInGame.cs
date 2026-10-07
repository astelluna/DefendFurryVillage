using System.Threading.Tasks;
using UnityEngine;

public class LoadingPanelInGame : MonoBehaviour
{
    //[SerializeField] StageManager 
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
        await CharacterManager.Instance.LoadSelectedCharacter();
        MyLog.Log("선택 캐릭터 초기화 완료");
        await CharacterManager.Instance.InitalizeCharacterTask;
        MyLog.Log("캐릭터 초기화 완료");
        await CharacterManager.Instance.IngameCharacterLoadTask;
        MyLog.Log("캐릭터 로딩 완료");
        await StageManager.Instance.StageInitTask;
        MyLog.Log("스테이지 초기화 완료");
        UnShowLoadingPanel();
        AugmentManager.Instance.SetAndShowAugmentUI(1);
        StageManager.Instance.WaveStart();

    }

}
