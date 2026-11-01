using UnityEngine;

public class FloorObject : MonoBehaviour
{
    [SerializeField] Renderer floorRenderer;
    private void OnEnable()
    {
        //인게임에서 월드별로 바닥 Material을 적용함
        Material material = StageManager.Instance.GetFloorData();
        if(material != null)
        {
            floorRenderer.sharedMaterial = material;
        }
    }
}
