using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Toggle vSync;
    //[SerializeField]
    //private Toggle fullscreen;
    [SerializeField]
    private TMP_Dropdown fullscreenDropdown;
    [SerializeField]
    private TMP_Dropdown framerateDropdown;
    [SerializeField]
    private TextMeshProUGUI framerateText;

    private void Awake()
    {
        SetVSync(1);
        //fullscreen.isOn = true;
        fullscreenDropdown.value = 0;
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

    public int GetFullscreen()
    {
        //return fullscreen.isOn;
        return fullscreenDropdown.value;
    }

    public void SetFullScreen()
    {
        int index = fullscreenDropdown.value + 1;
        switch (index)
        {
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;


        }
        //Screen.fullScreen = fullscreen.isOn;
        //Screen.fullScreenMode = 
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

    public void SetFromSave(int sync, int screenIndex, int dropdownValue)
    {
        SetVSync(sync);
        //fullscreen.isOn = screen;
        fullscreenDropdown.value = screenIndex;
        SetFullScreen();
        SetFramerateFromSave(dropdownValue);
    }

}
