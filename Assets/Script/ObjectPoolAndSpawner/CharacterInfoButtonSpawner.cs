using UnityEngine;

public class CharacterInfoButtonSpawner : MonoBehaviour
{
    public static CharacterInfoButtonSpawner Instance { get; private set; }
    [SerializeField] CharacterInfoButton characterInfoButton;
    [SerializeField] int poolSize = 15;

    ObjPool<CharacterInfoButton> infoButtonPool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        infoButtonPool = new ObjPool<CharacterInfoButton>(characterInfoButton, poolSize, gameObject.transform);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public CharacterInfoButton GetPrefabByPool(Transform parentTransform)
    {
        CharacterInfoButton characterinfoButtonPrefab = infoButtonPool.GetObject();
        characterinfoButtonPrefab.transform.SetParent(parentTransform, false);
        return characterinfoButtonPrefab;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    public void ReturnPrefabToPool(CharacterInfoButton infoButton)
    {
        infoButton.transform.SetParent(this.transform, false);
        infoButtonPool.ReturnObject(infoButton);
    }
}
