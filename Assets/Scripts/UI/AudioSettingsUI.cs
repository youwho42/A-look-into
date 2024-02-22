using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider fxSlider;
    public Slider animalsSlider;


    public AudioMixer audioMixer;


    private void Start()
    {
        InitializeVolumeSettings();
    }


    public AudioMixerGroup[] AllMixerGroups
    {
        get
        {
            return audioMixer.FindMatchingGroups(string.Empty);
        }
    }

    public void ChangeMasterVolume()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        //PlayerPrefs.SetFloat("Master", masterSlider.value);
    }
    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        //PlayerPrefs.SetFloat("Music", musicSlider.value);
    }
    public void ChangeAnimalVolume()
    {
        audioMixer.SetFloat("Animals", Mathf.Log10(animalsSlider.value) * 20);
        //PlayerPrefs.SetFloat("Animals", animalsSlider.value);
    }
    public void ChangeEffectsVolume()
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(fxSlider.value) * 20);
        //PlayerPrefs.SetFloat("Effects", fxSlider.value);
    }

    void InitializeVolumeSettings()
    {
        foreach (var group in AllMixerGroups)
        {
            if (!PlayerPrefs.HasKey(group.name))
                PlayerPrefs.SetFloat(group.name, 0.75f);
            else
                audioMixer.SetFloat(group.name, Mathf.Log10(PlayerPrefs.GetFloat(group.name) * 20));

        }
        SetSliders();


    }

    void SetSliders()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master");
        musicSlider.value = PlayerPrefs.GetFloat("Music");
        fxSlider.value = PlayerPrefs.GetFloat("Effects");
        animalsSlider.value = PlayerPrefs.GetFloat("Animals");
    }


}
