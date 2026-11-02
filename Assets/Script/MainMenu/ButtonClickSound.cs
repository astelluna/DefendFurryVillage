using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    [SerializeField] Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(PlaySound);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        SoundManager.Instance.PlaySFX(audioClip);
    }


}
