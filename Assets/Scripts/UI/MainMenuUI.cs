using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    UIScreen screen;
    [HideInInspector]
    public List<Button> allButtons = new List<Button>();
    string lastSaveName = "";
    public Button continueButton;
    public Button loadButton;
    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.MainMenuUI);
        allButtons = GetComponentsInChildren<Button>().ToList();
        
    }

    private void OnEnable()
    {
        SetContinueButton();
    }

    void SetContinueButton()
    {
        lastSaveName = "";
        if (Directory.Exists(Application.persistentDataPath))
        {
            string saveFolder = Application.persistentDataPath;
            DirectoryInfo d = new DirectoryInfo(saveFolder);
            List<FileInfo> files = new List<FileInfo>();
            foreach (var file in d.GetFiles("*.ali"))
            {
                files.Add(file);
            }
            if (files.Count > 0)
            {
                List<FileInfo> orderedList = files.OrderBy(x => x.LastWriteTime).ToList();
                orderedList.Reverse();
                lastSaveName = GetSaveName(orderedList[0].Name);
            }
                
            
        }

        continueButton.interactable = lastSaveName != "";
        loadButton.interactable = lastSaveName != "";
        
        
    }

    string GetSaveName(string fullName)
    {
        string[] name = fullName.Split('_');
        return name[0];
    }
    public void ContinueLastGame()
    {
        LevelManager.instance.LoadCurrentGame("MainScene", lastSaveName);
        
    }
    public void StartNewGame()
    {
        var backSelect = GetComponent<SetButtonSelected>();
        UIScreenManager.instance.DisplayWarning("New Game Warning", UIScreenType.CharacterSelectUI, LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Start"), backSelect);
        
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewOptions()
    {
        UIScreenManager.instance.DisplayOptionsUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewCredits()
    {
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayIngameUI(UIScreenType.CreditsUI, true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void ActivateButtons(bool isActive)
    {
        foreach (var butt in allButtons)
        {
            butt.interactable = isActive;
        }
        if(isActive)
            SetContinueButton();
    }
}
