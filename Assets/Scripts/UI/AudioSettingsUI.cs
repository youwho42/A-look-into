using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public static AudioSettingsUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider fxSlider;
    public Slider animalsSlider;


    public AudioMixer audioMixer;


    private void Start()
    {
        InitializeVolumeSettings();
        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeEffectsVolume();
        ChangeAnimalVolume();
    }


    public AudioMixerGroup[] AllMixerGroups
    {
        get
        {
            return audioMixer.FindMatchingGroups(string.Empty);
        }
    }
    public void Mute()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(0.001f) * 20);
    }

    public void ChangeMasterVolume()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
    }
    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
    }
    public void ChangeAnimalVolume()
    {
        audioMixer.SetFloat("Animals", Mathf.Log10(animalsSlider.value) * 20);
    }
    public void ChangeEffectsVolume()
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(fxSlider.value) * 20);
    }

    void InitializeVolumeSettings()
    {
        SetSliders(0.75f, 0.75f, 0.75f, 0.75f);
    }

    void SetSliders(float master, float music, float effects, float animals)
    {
        masterSlider.value = master;
        musicSlider.value = music;
        fxSlider.value = effects;
        animalsSlider.value = animals;
        
    }

    public void SetFromSave(float master, float music, float effects, float animals)
    {
        SetSliders(master, music, effects, animals);
        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeEffectsVolume();
        ChangeAnimalVolume();
    }
}
