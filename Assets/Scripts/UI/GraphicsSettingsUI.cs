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

    public void ToggleVSync()
    {
        int sync = vSync.isOn ? 1 : 0;
        QualitySettings.vSyncCount = sync;
        //PlayerPreferencesManager.instance.SaveVSync(QualitySettings.vSyncCount);
    }
    public void SetVSync(int sync)
    {
        QualitySettings.vSyncCount = sync;
        vSync.isOn = sync == 0 ? false : true;
    }

    public int GetVSync()
    {
        return QualitySettings.vSyncCount;
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = fullscreen.isOn;
    }


}
