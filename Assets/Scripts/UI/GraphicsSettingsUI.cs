using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Toggle vSync;
    [SerializeField]
    private Toggle fullscreen;

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
    }
    public void SetVSync(int sync)
    {
        QualitySettings.vSyncCount = sync;
        vSync.isOn = sync == 1;
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

    public void SetFromSave(int sync, bool screen)
    {
        SetVSync(sync);
        fullscreen.isOn = screen;
        SetFullScreen();
    }

}
