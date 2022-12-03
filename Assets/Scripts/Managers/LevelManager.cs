using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if (Application.isPlaying && loadTerrains)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TerrainDecoraion-StartArea", LoadSceneMode.Additive);
        }
    }

    public bool loadTerrains;

    [SerializeField]
    private CinemachineVirtualCamera startCam;

    [SerializeField]
    private GameObject loadScreen;
    [SerializeField]
    private GameObject titleMenu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject savedAlert;
    [SerializeField]
    private GameObject newGameWarning;
    [SerializeField]
    private GameObject controlsPanel;
    [SerializeField]
    private Toggle vSync;

    [SerializeField]
    private Slider loadScreenSlider;

    [SerializeField]
    private TextMeshProUGUI text;

    public UnityEvent EventLevelLoaded;

    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }
    private void Start()
    {
        PlayerInformation.instance.TogglePlayerInput(false);
    }
    public void StartNewGame(string levelName)
    {
        SavingLoading.instance.Save();
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);

        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);

        PlayerInformation.instance.TogglePlayerInput(true);
        RealTimeDayNightCycle.instance.isPaused = false;
        EventLevelLoaded.Invoke();
    }

    public void SetVSync()
    {
        if (QualitySettings.vSyncCount == 0)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void CancelNewGame()
    {
        newGameWarning.SetActive(false);
    }

    public void NewGameWarning()
    {
        newGameWarning.SetActive(true);
    }

    public void LoadCurrentGame(string levelName)
    {
        LoadLevel(levelName);
    }

    public void SaveGame()
    {
        SavingLoading.instance.Save();
        savedAlert.SetActive(true);
    }

    private void ChangeLevel(string levelName)
    {
        StartCoroutine(ChangeLevelCo(levelName));
    }

    private void LoadLevel(string levelName)
    {
        StartCoroutine(LoadLevelCo(levelName));
    }

    public void Pause(bool isPaused)
    {
        RealTimeDayNightCycle.instance.isPaused = isPaused;
        if (isPaused)
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayScreen(UIScreenType.PauseScreen);
            PlayerInformation.instance.TogglePlayerInput(false);
        }
        else
        {
            UIScreenManager.instance.HideScreens(UIScreenType.PauseScreen);
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            PlayerInformation.instance.TogglePlayerInput(true);
        }
        savedAlert.SetActive(false);
        HideControls();
    }

    public void ViewControls()
    {
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator ChangeLevelCo (string levelName)
    {
        AsyncOperation currentLevelLoading =  SceneManager.LoadSceneAsync(levelName);
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.LoadScreen);
        while (!currentLevelLoading.isDone)
        {
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }

        UIScreenManager.instance.HideScreens(UIScreenType.LoadScreen);

    }

    IEnumerator LoadLevelCo(string levelName)
    {
        UIScreenManager.instance.HideScreens(UIScreenType.PauseScreen);
        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName);

        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);

        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.LoadScreen);
        PlayerInformation.instance.TogglePlayerInput(false);
        while (!currentLevelLoading.isDone)
        {
            
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading scene: {Mathf.RoundToInt(progress * 100)}%";

            
            yield return null;
        }

        text.text = "Loading data from save.";
        // Something needs to be done about this. the scene is shown as loaded (because it is) at this point,
        // but the data still loads after this... figure it out?
        SavingLoading.instance.Load();

        yield return new WaitForSeconds(0.5f);

        text.text = "Thank you for waiting.";
        EventLevelLoaded.Invoke();

        GameObject player = FindObjectOfType<PlayerInput>().gameObject;
        var cams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var cam in cams)
        {
            if (cam.CompareTag("StartCam"))
            {
                cam.Priority = 0;
            }
        }
        yield return new WaitForSeconds(3f);
        Pause(false);
        UIScreenManager.instance.HideScreens(UIScreenType.LoadScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        
        DissolveEffect.instance.StartDissolve(player.GetComponentInChildren<SpriteRenderer>().material, 2f, true);
        yield return new WaitForSeconds(0.01f);
        PlayerInformation.instance.TogglePlayerInput(true);
    }


}
