using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HorizontalFovLock : MonoBehaviour
{
    public float horizontalFov = 36f;
    Camera cam;
    float lastAspect;

    void Awake() { cam = GetComponent<Camera>(); Apply(); }

    void Update()
    {
        float aspect = (float)Screen.width / Screen.height;
        if (!Mathf.Approximately(aspect, lastAspect)) Apply();
    }

    void Apply()
    {
        lastAspect = (float)Screen.width / Screen.height;
        float h = horizontalFov * Mathf.Deg2Rad;
        cam.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(h * 0.5f) / lastAspect) * Mathf.Rad2Deg;
    }
}