using UnityEngine;
[CreateAssetMenu(menuName = "Item/ItemData")]
public abstract class ItemDataSO : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string Desc { get; private set; }
    [field: SerializeField] public Sprite SpriteImage { get; private set; }
    public abstract eItemtype Itemtype { get; }
}
