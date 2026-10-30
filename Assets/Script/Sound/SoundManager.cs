using UnityEngine;
using UnityEngine.Audio;

public enum eVolumeType
{
    Master, BGM, SFX, Voice
}

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
        foreach(eVolumeType type in System.Enum.GetValues(typeof(eVolumeType)))
        {
            SetVolume(type, GetVolume(type));
        }
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
    // enum → 믹서 파라미터 이름 ("BGM" → "BGMVol")
    private static string ParamName(eVolumeType type) => type + "Vol";
    // ---------- 볼륨 (설정창 슬라이더가 호출) ----------
    public float GetVolume(eVolumeType type)
    {
        return PlayerPrefs.GetFloat(ParamName(type), 1f);
    }
    public void SetVolume(eVolumeType type, float value)
    {
        string param = ParamName(type);
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(param, value);
    }


}