using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("AudioSources (각각 Output에 해당 그룹 지정)")]
    public AudioSource bgmSource;    // Output: BGM 그룹
    public AudioSource sfxSource;    // Output: SFX 그룹
    public AudioSource voiceSource;  // Output: Voice 그룹
    protected override void OnAwake()
    {
    }
    void Start()
    {
        // 저장된 볼륨 적용
        SetVolume("MasterVol", PlayerPrefs.GetFloat("MasterVol", 1f));
        SetVolume("BGMVol", PlayerPrefs.GetFloat("BGMVol", 1f));
        SetVolume("SFXVol", PlayerPrefs.GetFloat("SFXVol", 1f));
        SetVolume("VoiceVol", PlayerPrefs.GetFloat("VoiceVol", 1f));
    }

    // ---------- 재생 ----------
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);   // 겹쳐서 재생 가능
    }

    public void PlayVoice(AudioClip clip)
    {
        //voiceSource.Stop();            // 음성은 겹치면 안 되니까 끊고 재생
        //voiceSource.clip = clip;
        voiceSource.PlayOneShot(clip);
    }

    // ---------- 볼륨 (설정창 슬라이더가 호출) ----------
    public void SetVolume(string param, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(param, value);
    }


}