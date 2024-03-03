using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;

public class LoadableSaveButton : MonoBehaviour
{
    public TextMeshProUGUI loadableSaveName;
    public string loadFileName;

    public void SetCurrentLoadFile()
    {
        if(!LoadSelectionUI.instance.warningActive)
            LoadSelectionUI.instance.SetCurrenLoadFileName(loadFileName);
    }

    public void SetLoadButton(FileInfo file)
    {
        string[] name = file.Name.Split('_');
        var format = new CultureInfo(CultureInfo.CurrentCulture.Name);
        string date = file.LastWriteTime.ToString("d", format);
        string time = file.LastWriteTime.ToString("t", format);
        loadableSaveName.text = $"{name[0]} - {date} - {time}";
        loadFileName = name[0];
    }

    public void Clear()
    {
        loadableSaveName.text = "";
        loadFileName = "";
    }

}
