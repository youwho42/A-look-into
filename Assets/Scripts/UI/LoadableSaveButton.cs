using System.Collections;
using System.Collections.Generic;
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

    public void SetLoadButton(string fileName)
    {
        string[] name = fileName.Split('_');
        loadableSaveName.text = name[0];
        loadFileName = name[0];
    }

    public void Clear()
    {
        loadableSaveName.text = "";
        loadFileName = "";
    }

    public void DeleteSave()
    {
        if (LoadSelectionUI.instance.warningActive)
            return;
        string path = $"{Application.persistentDataPath}/{loadFileName}_save.ali";
        LoadSelectionUI.instance.DisplayDeleteWarning(path);
    }

    
    
}
