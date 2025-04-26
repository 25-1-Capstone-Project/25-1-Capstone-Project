using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{   
    [System.Serializable]
    public class Sound
    {
        public string key;
        public AudioClip clip;
    }

    public List<Sound> sounds = new();

    private Dictionary<string, AudioClip> soundDict;

    private void OnEnable()
    {
        soundDict = new();
        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.key))
                soundDict.Add(sound.key, sound.clip);
        }
    }

    public AudioClip GetClip(string key)
    {
        if (soundDict != null && soundDict.TryGetValue(key, out var clip))
            return clip;
        
        Debug.LogWarning($"Sound key '{key}' not found!");
        return null;
    }
}