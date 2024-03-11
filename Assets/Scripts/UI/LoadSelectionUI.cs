using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadSelectionUI : MonoBehaviour
{
    public static LoadSelectionUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject loadButtonHolder;
    public LoadableSaveButton loadableSaveButton;

    List<LoadableSaveButton> loadableSaveButtons = new List<LoadableSaveButton>();
    public GameObject backButton;
    public Button loadButton;
    public Button deleteButton;

    UIScreenType backButtonScreen;

    public GameObject deleteWarning;
    public GameObject deleteAllWarning;
    string fileDeletePath;

    public bool warningActive;
    public string currentLoadFileName { get; private set; }

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.LoadGameUI);
    
        gameObject.SetActive(false);
    }
    

    private void OnEnable()
    {
        GameEventManager.onGameSavedEvent.AddListener(SetAvailableLoads);
        HideDeleteWarning(deleteWarning);
        HideDeleteWarning(deleteAllWarning);
        SetAvailableLoads();
        RefreshButtonsValid();
    }
    private void OnDisable()
    {
        GameEventManager.onGameSavedEvent.RemoveListener(SetAvailableLoads);
    }
    public void SetBackButton(UIScreenType screen)
    {
        backButtonScreen = screen;
    }

    public void LoadGameFile()
    {
        if (!warningActive)
            LevelManager.instance.LoadCurrentGame("MainScene", currentLoadFileName);
    }

    public void BackButton()
    {
        if (!warningActive)
        {
            UIScreenManager.instance.HideScreenUI();
            UIScreenManager.instance.DisplayScreenUI(backButtonScreen, true);
            ClearCurrentLoadFileName();
        }
            
    }
    public void SetAvailableLoads()
    {
        ClearLoadableSavesButtons();
        
        if (Directory.Exists(Application.persistentDataPath))
        {
            string saveFolder = Application.persistentDataPath;

            DirectoryInfo d = new DirectoryInfo(saveFolder);
            List<FileInfo> files = new List<FileInfo>();
            foreach (var file in d.GetFiles("*.ali"))
            {
                files.Add(file);
            }
            //sort using date created
            List<FileInfo> orderedList = files.OrderBy(x => x.LastWriteTime).ToList();
            orderedList.Reverse();
            CreateLoadButtons(orderedList);
        }
        else
        {
            File.Create(Application.persistentDataPath);
            return;
        }
    }

    void CreateLoadButtons(List<FileInfo> files)
    {
        foreach (var file in files)
        {
            LoadableSaveButton newLoadableSave = Instantiate(loadableSaveButton, loadButtonHolder.transform);
            loadableSaveButtons.Add(newLoadableSave);
            newLoadableSave.SetLoadButton(file);
        }
    }
    public void RefreshButtonsValid()
    {
        if (currentLoadFileName == "")
        {
            loadButton.interactable = false;
            deleteButton.interactable = false;
        }
        else
        {
            loadButton.interactable = true;
            deleteButton.interactable = true;
        }
    }
    public void SetCurrenLoadFileName(string name)
    {
        currentLoadFileName = name;
        RefreshButtonsValid();
    }

    public void ClearCurrentLoadFileName()
    {
        currentLoadFileName = "";
        RefreshButtonsValid();
    }

    public void ClearLoadableSavesButtons()
    {
        foreach (Transform child in loadButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        loadableSaveButtons.Clear();
    }
    public void DeleteSave()
    {
        if (warningActive)
            return;
        string path = $"{Application.persistentDataPath}/{currentLoadFileName}_save.ali";
        DisplayDeleteWarning(deleteWarning, path); 
    }

    public void DeleteAllSaves()
    {
        if (warningActive)
            return;
        
        DisplayDeleteWarning(deleteAllWarning);
    }

    public void DisplayDeleteWarning(GameObject warning, string path = "")
    {
        warning.SetActive(true);
        fileDeletePath = path;
        warningActive = true;
        warning.GetComponent<SetButtonSelected>().SetSelectedButton();
    }
    public void HideDeleteWarning(GameObject warning)
    {
        warning.SetActive(false);
        ClearCurrentLoadFileName();
        warningActive = false;
        gameObject.GetComponent<SetButtonSelected>().SetSelectedButton();
    }

    public void DeleteSaveFile()
    {
        
        SavingLoading.instance.DeleteFile(fileDeletePath);
        string versionPath = $"{Application.persistentDataPath}/{currentLoadFileName}Version_save.aliv";
        SavingLoading.instance.DeleteFile(versionPath);
        SetAvailableLoads();
        ClearCurrentLoadFileName();
        HideDeleteWarning(deleteWarning);
    }
    public void DeleteAllSaveFiles()
    {
        string saveFolder = Application.persistentDataPath;
        string[] filePaths = Directory.GetFiles(saveFolder);
        foreach (string filePath in filePaths)
        {
            if (filePath.EndsWith(".alio") || filePath.EndsWith(".alit") || filePath.EndsWith(".log"))
                continue;
            SavingLoading.instance.DeleteFile(filePath);
            
        }

        SetAvailableLoads();
        ClearCurrentLoadFileName();
        HideDeleteWarning(deleteAllWarning);
    }
}
