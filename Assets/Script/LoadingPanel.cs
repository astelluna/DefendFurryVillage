using System.Threading.Tasks;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
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
        Invoke(nameof(TestFunc), 5);

    }
    private async void TestFunc()
    {
        ShowLoadingPanel();
        await CharacterManager.Instance.InitalizeCharacterTask;
        UnShowLoadingPanel();
        AugmentManager.Instance.SetAndShowAugmentUI(1);
    }

}
