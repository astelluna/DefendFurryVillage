using UnityEngine;

public class LoadingPanelMainScene : MonoBehaviour
{
    //메인씬에서 로딩을 관리.

    private async void Start()
    {
        this.gameObject.SetActive(true);
        await CharacterManager.Instance.InitalizeCharacterTask;
        this.gameObject.SetActive(false);
    }
}
