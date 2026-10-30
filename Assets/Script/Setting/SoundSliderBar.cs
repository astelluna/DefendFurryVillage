using UnityEngine;
using UnityEngine.UI;

public class SoundSliderBar : MonoBehaviour
{
    [SerializeField] private eVolumeType type;
    [SerializeField] private Slider slider;

    private void Start()
    {
        // 저장된 값으로 핸들 위치 맞추기 (이벤트 발생 없이)
        slider.SetValueWithoutNotify(SoundManager.Instance.GetVolume(type));
        slider.onValueChanged.AddListener(ValueChange);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(ValueChange);
    }

    private void ValueChange(float amount)
    {
        SoundManager.Instance.SetVolume(type, amount);
    }

}
