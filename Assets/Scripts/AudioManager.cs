using UnityEngine;
using System.Collections.Generic;


public class AudioManager : Singleton<AudioManager> 
{

    [SerializeField] private int sfxSourceCount = 5;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private SoundLibrary soundLibrary;

    private List<AudioSource> sfxSources;
    private int currentSfxIndex = 0;

    protected override void Awake()
    {
        base.Awake(); 
        sfxSources = new List<AudioSource>();

        for (int i = 0; i < sfxSourceCount; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxSources.Add(source);
        }
    }

    public void PlaySFX(string key)
    {
        var clip = soundLibrary.GetClip(key);
        if (clip != null)
        {
            sfxSources[currentSfxIndex].PlayOneShot(clip);
            currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Count;
        }
    }

    // BGM 관련은 그대로
    public void PlayBGM(AudioClip bgmClip, bool loop = true)
    {
        if (bgmSource.clip == bgmClip) return;
        bgmSource.clip = bgmClip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}