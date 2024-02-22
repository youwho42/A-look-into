using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class PlayerPreferencesManager : MonoBehaviour
{
    public static PlayerPreferencesManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider fxSlider;
    public Slider animalsSlider;


    public AudioMixer audioMixer;

    public AudioMixerGroup[] AllMixerGroups
    {
        get
        {
            return audioMixer.FindMatchingGroups(string.Empty);
        }
    }

    

    private IEnumerator Start()
    {
        InitializeVolumeSettings();
        if (!PlayerPrefs.HasKey("Vsync"))
            PlayerPrefs.SetInt("Vsync", 0);
        if (!PlayerPrefs.HasKey("AutoZoomReset"))
            PlayerPrefs.SetInt("AutoZoomReset", 1);

        if (!PlayerPrefs.HasKey("DisplayHUD"))
            PlayerPrefs.SetInt("DisplayHUD", 1);
        yield return new WaitForSeconds(2);
        //LevelManager.instance.SetVSync(PlayerPrefs.GetInt("Vsync"));
        //LevelManager.instance.SetAutoZoomReset(PlayerPrefs.GetInt("AutoZoomReset"));
        //LevelManager.instance.SetDisplayHUD(PlayerPrefs.GetInt("DisplayHUD"));
        
    }

    public void SaveVSync(int sync)
    {
        PlayerPrefs.SetInt("Vsync", sync);
    }

    public void SaveAutoZoomReset(int autoReset)
    {
        PlayerPrefs.SetInt("AutoZoomReset", autoReset);
    }

    public void SaveDisplayHUD(int hud)
    {
        PlayerPrefs.SetInt("DisplayHUD", hud);
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
    public void ChangeMasterVolume()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        PlayerPrefs.SetFloat("Master", masterSlider.value);
    }
    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }
    public void ChangeAnimalVolume()
    {
        audioMixer.SetFloat("Animals", Mathf.Log10(animalsSlider.value) * 20);
        PlayerPrefs.SetFloat("Animals", animalsSlider.value);
    }
    public void ChangeEffectsVolume()
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(fxSlider.value) * 20);
        PlayerPrefs.SetFloat("Effects", fxSlider.value);
    }

}
