using UnityEngine;
[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemDataSO : ScriptableObject
{
    //그냥 현재는 아이템 안쓰니까 형식만 만들어두자. 사용하면 사용하는 그런 코드가 있을거임 
    [Header("아이템 정보")]
    [field: SerializeField] public string ItemId { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public Sprite ItemImage { get; private set; }

}
