using UnityEngine;

public class EquipmentButtonSpawner : MonoBehaviour
{
    public static EquipmentButtonSpawner Instance { get; private set; }
    [SerializeField] EquipmentUI equipmentPrefab;
    [SerializeField] int poolSize = 15;

    ObjPool<EquipmentUI> infoButtonPool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        infoButtonPool = new ObjPool<EquipmentUI>(equipmentPrefab, poolSize, gameObject.transform);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public EquipmentUI GetPrefabByPool(Transform parentTransform)
    {
        EquipmentUI equipmentUI = infoButtonPool.GetObject();
        equipmentUI.transform.SetParent(parentTransform, false);
        return equipmentUI;
    }
    //오브젝트 반환 (반환이벤트없으면 public으로 써도됨)
    public void ReturnPrefabToPool(EquipmentUI infoButton)
    {
        infoButton.transform.SetParent(this.transform, false);
        infoButtonPool.ReturnObject(infoButton);
    }
}