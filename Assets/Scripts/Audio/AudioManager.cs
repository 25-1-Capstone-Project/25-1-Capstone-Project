using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private int sfxSourceCount = 5;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private SoundLibrary soundLibrary;
    [SerializeField] private GameObject sfxParent;
    [SerializeField] private AudioMixer audioMixer;
    private AudioSource[] sfxSources;
    private int currentSfxIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        sfxSources = sfxParent.GetComponents<AudioSource>();

    }

    public void PlaySFX(string key)
    {
        var clip = soundLibrary.GetClip(key);
        if (clip != null)
        {
            sfxSources[currentSfxIndex].PlayOneShot(clip);
            currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Length;
        }
    }

    public void PlayBGM(string key, bool loop = true)
    {
        var clip = soundLibrary.GetClip(key);
        if (bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
}