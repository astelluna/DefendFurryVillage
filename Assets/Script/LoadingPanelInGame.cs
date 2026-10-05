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
        await CharacterManager.Instance.InitalizeCharacterTask;
        await StageManager.Instance.StageInitTask;
        UnShowLoadingPanel();
        AugmentManager.Instance.SetAndShowAugmentUI(1);
        StageManager.Instance.WaveStart();

    }

}
