using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public Button loadButton;

    public GameObject deleteWarning;
    string fileDeletePath;

    public bool warningActive;
    public string currentLoadFileName { get; private set; }
    private void Start()
    {
        GameEventManager.onGameSavedEvent.AddListener(SetAvailableLoads);
        GameEventManager.onGameLoadedEvent.AddListener(HideDeleteWarning);
    }
    private void OnDisable()
    {
        GameEventManager.onGameSavedEvent.RemoveListener(SetAvailableLoads);
        GameEventManager.onGameLoadedEvent.RemoveListener(HideDeleteWarning);
    }

    private void OnEnable()
    {
        HideDeleteWarning();
        SetAvailableLoads();
        RefreshLoadButtonValid();
    }
    
    public void LoadGameFile()
    {
        if (!warningActive)
            LevelManager.instance.LoadCurrentGame("MainScene", currentLoadFileName);
    }

    public void BackButton()
    {
        if (!warningActive)
            LevelManager.instance.LoadFileBackButton();
    }
    public void SetAvailableLoads()
    {
        ClearLoadableSavesButtons();
         
        if (Directory.Exists(Application.persistentDataPath))
        {
            string saveFolder = Application.persistentDataPath;

            DirectoryInfo d = new DirectoryInfo(saveFolder);
            foreach (var file in d.GetFiles("*.ali"))
            {
                
                LoadableSaveButton newLoadableSave = Instantiate(loadableSaveButton, loadButtonHolder.transform);
                loadableSaveButtons.Add(newLoadableSave);
                newLoadableSave.SetLoadButton(file.Name);
            }
        }
        else
        {
            File.Create(Application.persistentDataPath);
            return;
        }
    }
    public void RefreshLoadButtonValid()
    {
        if (currentLoadFileName == "")
            loadButton.interactable = false;
        else
            loadButton.interactable = true;
    }
    public void SetCurrenLoadFileName(string name)
    {
        currentLoadFileName = name;
        RefreshLoadButtonValid();
    }

    public void ClearCurrentLoadFileName()
    {
        currentLoadFileName = "";
        RefreshLoadButtonValid();
    }

    public void ClearLoadableSavesButtons()
    {
        while (loadButtonHolder.transform.childCount > 0)
        {
            DestroyImmediate(loadButtonHolder.transform.GetChild(0).gameObject);
        }
        loadableSaveButtons.Clear();
        HideDeleteWarning();
    }

    public void DisplayDeleteWarning(string path = "")
    {
        deleteWarning.SetActive(true);
        fileDeletePath = path;
        warningActive = true;
    }
    public void HideDeleteWarning()
    {
        deleteWarning.SetActive(false);
        ClearCurrentLoadFileName();
        warningActive = false;
    }

    public void DeleteSaveFile()
    {
        SavingLoading.instance.DeleteFile(fileDeletePath);
        SetAvailableLoads();
        ClearCurrentLoadFileName();
        HideDeleteWarning();
    }
}
