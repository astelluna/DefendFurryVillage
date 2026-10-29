using UnityEngine;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMasterVolume(float value) => SetVolume("MasterVol", value);
    public void SetBGMVolume(float value) => SetVolume("BGMVol", value);
    public void SetSFXVolume(float value) => SetVolume("SFXVol", value);
    public void SetVoiceVolume(float value) => SetVolume("VoiceVol", value);

    void SetVolume(string param, float value)
    {
        // 슬라이더 0~1 값을 데시벨(-80 ~ 0)로 변환
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(param, value); // 저장
    }

    void Start()
    {
        // 저장된 값 불러오기 (없으면 1)
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVol", 1f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVol", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVol", 1f));
        SetVoiceVolume(PlayerPrefs.GetFloat("VoiceVol", 1f));
    }
}