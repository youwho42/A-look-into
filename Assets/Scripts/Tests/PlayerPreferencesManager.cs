using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AudioTrack
{
    Music,
    Effects,
    Animals
}
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

    public Slider musicSlider;
    public Slider fxSlider;
    public Slider animalsSlider;

    private void Start()
    {
        InitializeVolumeSettings();
        if (!PlayerPrefs.HasKey("Vsync"))
            PlayerPrefs.SetInt("Vsync", 0);
        if (!PlayerPrefs.HasKey("AutoZoomReset"))
            PlayerPrefs.SetInt("AutoZoomReset", 1);

        if (!PlayerPrefs.HasKey("DisplayHUD"))
            PlayerPrefs.SetInt("DisplayHUD", 0);

        LevelManager.instance.SetVSync(PlayerPrefs.GetInt("Vsync"));
        LevelManager.instance.SetAutoZoomReset(PlayerPrefs.GetInt("AutoZoomReset"));
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
        PlayerPrefs.SetInt("AutoZoomReset", hud);
    }

    void InitializeVolumeSettings()
    {
        if(!PlayerPrefs.HasKey(AudioTrack.Music.ToString()))
            PlayerPrefs.SetFloat(AudioTrack.Music.ToString(), 0.5f);
        if (!PlayerPrefs.HasKey(AudioTrack.Effects.ToString()))
            PlayerPrefs.SetFloat(AudioTrack.Effects.ToString(), 0.5f);
        if (!PlayerPrefs.HasKey(AudioTrack.Animals.ToString()))
            PlayerPrefs.SetFloat(AudioTrack.Animals.ToString(), 0.5f);

        musicSlider.value = GetTrackVolume(AudioTrack.Music);
        fxSlider.value = GetTrackVolume(AudioTrack.Effects);
        animalsSlider.value = GetTrackVolume(AudioTrack.Animals);
    }

    public void SetTrackVolume(AudioTrack track, float amount)
    {
        PlayerPrefs.SetFloat(track.ToString(), amount);
        GameEventManager.onVolumeChangedEvent.Invoke();
    }
    public float GetTrackVolume(AudioTrack track)
    {
        return PlayerPrefs.GetFloat(track.ToString());
    }


    public void ChangeMusicVolume()
    {
        SetTrackVolume(AudioTrack.Music, musicSlider.value);
    }
    public void ChangeEffectsVolume()
    {
        SetTrackVolume(AudioTrack.Effects, fxSlider.value);
    }
    public void ChangeAnimalsVolume()
    {
        SetTrackVolume(AudioTrack.Animals, animalsSlider.value);
    }
}
