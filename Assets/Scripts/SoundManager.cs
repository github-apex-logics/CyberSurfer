using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<Clips> audioClips;
    private Dictionary<ClipName, Clips> clipDict;
    public AudioSource audioSource, bgSrc;
    public int poolSize = 5;
    private List<AudioSource> sfxSources = new List<AudioSource>();



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }


        audioSource = GetComponent<AudioSource>();
        clipDict = new Dictionary<ClipName, Clips>();
        foreach (var clip in audioClips)
        {
            if (!clipDict.ContainsKey(clip.clipType))
            {
                clipDict.Add(clip.clipType, clip);
            }

        }
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(src);
        }


        bgSrc.volume = PlayerPrefs.GetFloat("Music", 0.4f);
    }


    public void SetMusic(float value)
    {
        PlayerPrefs.SetFloat("Music", value);
        bgSrc.volume = PlayerPrefs.GetFloat("Music");
    }

    private void Update()
    {
       
    }

    public void PlayClip(ClipName clipName)
    {
        if (clipDict.TryGetValue(clipName, out Clips c))
        {
            AudioSource freeSource = GetAvailableAudioSource();
            if (freeSource != null)
            {
                freeSource.clip = c.clip;
                freeSource.pitch = c.pitch;
                freeSource.volume = c.volume;
                freeSource.loop = c.loop;
                freeSource.Play();
            }
        }
    }


    public void StopClip(ClipName clipName)
    {
        if (clipDict.TryGetValue(clipName, out Clips c))
        {
         
            audioSource.Stop();
        }

    }




    //for multiplayer
    public void PlayClip(ClipName clipName, bool network)
    {
        if (clipDict.TryGetValue(clipName, out Clips c))
        {
            audioSource.clip = c.clip;
            audioSource.pitch = c.pitch;
            audioSource.volume = c.volume;
            audioSource.loop = c.loop;
            audioSource.Play();
        }

    }




    private AudioSource GetAvailableAudioSource()
    {
        foreach (var src in sfxSources)
        {
            if (!src.isPlaying)
                return src;
        }

        // Optional: fallback to any source (will cut off sound)
        return sfxSources[0];
    }



}



public enum ClipName
{
    Jump,
    Slide,
    Coin,
    PowerUp,
    EnemyDie,
    Spawn,
    BossClap,
    BossGrunt,
    LevelComplete,
    Reward
}

[Serializable]
public class Clips
{
    public string name;
    public ClipName clipType;
    public AudioClip clip;
    public float pitch, volume;
    public bool loop;

}

