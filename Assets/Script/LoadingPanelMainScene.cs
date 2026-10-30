using UnityEngine;

public class LoadingPanelMainScene : MonoBehaviour
{
    //메인씬에서 로딩을 관리.
    public static LoadingPanelMainScene Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    private async void Start()
    {
        this.gameObject.SetActive(true);
        await CharacterManager.Instance.InitalizeCharacterTask;
        await AddressableManager.Instance.WaitPreloadAsync();
        this.gameObject.SetActive(false);
        AdsManager.Instance.ShowInterstitial(null);
    }

    public void ShowPanel(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }
}
