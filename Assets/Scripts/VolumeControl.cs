using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioSource[] SoundsSrc, MusicSrc;
    public Slider soundSlider, musicSlider,sensitivitySlider ;

    private void Start()
    {
        GetAudiosources();
        foreach (AudioSource src in SoundsSrc)
        {
            src.volume = PlayerPrefs.GetFloat("Sound",0.7f);

        }
        foreach (AudioSource src in MusicSrc)
        {
            src.volume = PlayerPrefs.GetFloat("Music",0.5f);
        }
        SetSliderValue();

    }

    private void OnEnable()
    {
        SetSliderValue();



        sensitivitySlider.minValue = 0.1f;
        sensitivitySlider.maxValue = 0.5f;

    }

    void GetAudiosources()
    {
        SoundsSrc = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
            .Where(src => src.gameObject.name != "BG_Music")
            .ToArray();

        MusicSrc = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
            .Where(src => src.gameObject.name == "BG_Music")
            .ToArray();
    }


    void SetSliderValue()
    {
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0.7f);
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.25f);
    }


    public void SetSound()
    {
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
        

        foreach (AudioSource src in SoundsSrc)
        {
            src.volume = PlayerPrefs.GetFloat("Sound");
        }
    }



    public void SetMusic()
    {
        //PlayerPrefs.SetFloat("Music", musicSlider.value);
        SoundManager.instance.SetMusic(musicSlider.value);

        foreach (AudioSource src in MusicSrc)
        {
            src.volume = PlayerPrefs.GetFloat("Music");
        }

    }

    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
    }


}
