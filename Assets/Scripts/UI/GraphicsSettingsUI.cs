using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Toggle vSync;
    [SerializeField]
    private Toggle fullscreen;
    [SerializeField]
    private TMP_Dropdown framerateDropdown;
    [SerializeField]
    private TextMeshProUGUI framerateText;

    private void Awake()
    {
        SetVSync(1);
        fullscreen.isOn = true;
        SetFullScreen();
    }
    public void ToggleVSync()
    {
        int sync = vSync.isOn ? 1 : 0;
        QualitySettings.vSyncCount = sync;
        ToggleFramerateVisuals();
    }
    public void SetVSync(int sync)
    {
        QualitySettings.vSyncCount = sync;
        vSync.isOn = sync == 1;
        ToggleFramerateVisuals();
    }

    public int GetVSync()
    {
        return QualitySettings.vSyncCount;
    }

    public bool GetFullscreen()
    {
        return fullscreen.isOn;
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = fullscreen.isOn;
    }


    public void SetFramerateFromSave(int value)
    {
        framerateDropdown.value = value;
        framerateDropdown.RefreshShownValue();
        SetFramerateLimit();
    }

    public void SetFramerateLimit()
    {
        int value = framerateDropdown.value;
        if (value == 0) 
            Application.targetFrameRate = -1;
        else if(value == 1)
            Application.targetFrameRate = 30;
        else if (value == 2)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = 120;
    }


    public int GetLimitedFramerate()
    {
        return framerateDropdown.value;
    }

    void ToggleFramerateVisuals()
    {
        framerateDropdown.interactable = !vSync.isOn;
        framerateText.color = new Color(0, 0, 0, vSync.isOn ? 0.5f : 1.0f);
    }

    public void SetFromSave(int sync, bool screen, int dropdownValue)
    {
        SetVSync(sync);
        fullscreen.isOn = screen;
        SetFullScreen();
        SetFramerateFromSave(dropdownValue);
    }

}
