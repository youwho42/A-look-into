using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using Klaxon.UndertakingSystem;

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

        
    }

    

    public bool loadTerrains;

    //[SerializeField]
    //private CinemachineVirtualCamera startCam;
    //[SerializeField]
    //private CinemachineVirtualCamera mainCam;
    
    [SerializeField]
    private GameObject loadScreen;
    [SerializeField]
    private GameObject titleMenu;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject savedAlert;
    [SerializeField]
    private GameObject newGameWarning;
    [SerializeField]
    private Toggle vSync;
    [SerializeField]
    private Toggle fullscreen;
    [SerializeField]
    private Toggle autoZoomReset;
    [HideInInspector]
    public int autoZoomBinary;
    [SerializeField]
    private Toggle HUDDisplay;
    [HideInInspector]
    public int HUDBinary;

    [SerializeField]
    private Slider loadScreenSlider;

    [SerializeField]
    private TextMeshProUGUI text;



    public bool isInCutscene;
    Material playerMaterial;
    bool inPauseMenu;
    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }
    private IEnumerator Start()
    {
        if (Application.isPlaying && loadTerrains)
        {

            if (!SceneManager.GetSceneByName("MainScene-Decoration").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Decoration", LoadSceneMode.Additive);
            }

        }

        PlayerInformation.instance.TogglePlayerInput(false);
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        isInCutscene = true;
        yield return new WaitForSeconds(0.2f);
        SelectStartButton();
        fullscreen.isOn = Screen.fullScreen;
    }
    public void SelectStartButton()
    {
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayScreen(UIScreenType.StartScreen);

    }
    
    public void StartNewGame(string levelName)
    {
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerSelect);
        NewGameWarning(false);
    }

    public void DisplayLoadFilesUI()
    {
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.LoadFileScreen);
        
    }
    public void LoadFileBackButton()
    {
        if (inPauseMenu)
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayScreen(UIScreenType.PauseScreen);
        }
        else
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayScreen(UIScreenType.StartScreen);
        }

    }

    public void PlayerSpriteAndNameAccepted()
    {
        isInCutscene = true;
        UIScreenManager.instance.HideScreens(UIScreenType.PlayerSelect);
        //StartCoroutine("StartNewGameCo");
        GameEventManager.onNewGameStartedEvent.Invoke();
        GameEventManager.onStatUpdateEvent.Invoke();
    }
    public void NewGamePlayerFadeIn()
    {
        StartCoroutine("PlayerFadeInCo");
    }
    public void ActivatePlayer()
    {
        isInCutscene = false;
        PlayerInformation.instance.TogglePlayerInput(true);
        GameEventManager.onPlayerPositionUpdateEvent.Invoke();
        if (HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
    }

    public void ToggleVSync()
    {
        if (vSync.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        PlayerPreferencesManager.instance.SaveVSync(QualitySettings.vSyncCount);
    }
    public void SetVSync(int sync)
    {
        QualitySettings.vSyncCount = sync;
        vSync.isOn = sync == 0 ? false : true;
    }
    public void ToggleAutoZoomReset()
    {
        autoZoomBinary = autoZoomReset.isOn? 1 : 0;
        PlayerPreferencesManager.instance.SaveAutoZoomReset(autoZoomBinary);
    }
    
    public void SetAutoZoomReset(int zoomBinary)
    {
        autoZoomBinary = zoomBinary;
        autoZoomReset.isOn = autoZoomBinary == 0 ? false : true;
    }
    public void ToggleDisplayHUD()
    {
        HUDBinary = HUDDisplay.isOn ? 1 : 0;
        PlayerPreferencesManager.instance.SaveDisplayHUD(HUDBinary);
    }
    public void SetDisplayHUD(int displayHUD)
    {
        HUDBinary = displayHUD;
        HUDDisplay.isOn = HUDBinary == 0 ? false : true;
        
    }
    public int GetVSync()
    {
        return QualitySettings.vSyncCount;
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = fullscreen.isOn;
    }

    public void CancelNewGame()
    {
        newGameWarning.SetActive(false);
        SelectStartButton();
    }

    public void NewGameWarning(bool active)
    {
        newGameWarning.SetActive(active);
        if (newGameWarning.TryGetComponent(out SetButtonSelected butt) && active)
            butt.SetSelectedButton();
    }

    public void LoadCurrentGame(string levelName, string loadFileName)
    {
        LoadLevel(levelName, loadFileName);
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

    private void LoadLevel(string levelName, string loadFileName)
    {
        StartCoroutine(LoadLevelCo(levelName, loadFileName));
    }
    public void LoadTitleScreen()
    {
        StartCoroutine(LoadTitleScreenCo("MainScene"));
    }

    public void Pause(bool isPaused)
    {
        inPauseMenu = isPaused;
        RealTimeDayNightCycle.instance.isPaused = isPaused;
        
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        if (isPaused)
        {
            UIScreenManager.instance.HideAllScreens();
            UIScreenManager.instance.DisplayScreen(UIScreenType.PauseScreen);
            UIScreenManager.instance.DisplayAdditionalUI(UIScreenType.PlayerUI);
            PlayerInformation.instance.TogglePlayerInput(false);
        }
        else
        {
            UIScreenManager.instance.HideScreens(UIScreenType.PauseScreen);
            if(HUDBinary == 1)
                UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
            PlayerInformation.instance.TogglePlayerInput(true);
        }
        savedAlert.SetActive(false);
        
    }

    public void ViewControls()
    {
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayScreen(UIScreenType.ControlsScreen);
    }

    public void ViewVideoSettings()
    {
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayScreen(UIScreenType.VideoScreen);
    }


    public void HideControls()
    {
        LoadFileBackButton();
    }
    public void ViewVolumeControls()
    {
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayScreen(UIScreenType.AudioScreen);
    }

    public void HideVolumeControls()
    {
        LoadFileBackButton();
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

    IEnumerator LoadLevelCo(string levelName, string loadFileName)
    {
        
        UIScreenManager.instance.HideScreens(UIScreenType.PauseScreen);
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.LoadScreen);
        PlayerInformation.instance.TogglePlayerInput(false);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();

        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName +"-Terrain");

        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);

        
        while (!currentLevelLoading.isDone)
        {
            
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";

            
            yield return null;
        }
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);

        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Decorations: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }



        text.text = "Loading data from save.";
        // Something needs to be done about this. the scene is shown as loaded (because it is) at this point,
        // but the data still loads after this... figure it out?
        SavingLoading.instance.Load(LoadSelectionUI.instance.currentLoadFileName);
        PlayerDistanceToggle.instance.PopulateAnimalList();
        Debug.Log("this far");
        yield return new WaitForSecondsRealtime(0.5f);

        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        
        GameEventManager.onGameLoadedEvent.Invoke();
        yield return new WaitForSecondsRealtime(3f);
       

        Time.timeScale = 1;
        UIScreenManager.instance.HideScreens(UIScreenType.LoadScreen);
        if(HUDBinary == 1)
            UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        
        yield return new WaitForSecondsRealtime(0.1f);
        DissolveEffect.instance.StartDissolve(playerMaterial, 2f, true);
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerInformation.instance.playerShadow.SetActive(true); 
        yield return new WaitForSecondsRealtime(0.5f);
        Pause(false);
        ActivatePlayer();
    }

    IEnumerator LoadTitleScreenCo(string levelName)
    {

        UIScreenManager.instance.HideScreens(UIScreenType.PauseScreen);
        UIScreenManager.instance.HideScreens(UIScreenType.StartScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.LoadScreen);
        PlayerInformation.instance.TogglePlayerInput(false);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();

        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Terrain");

        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);

        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Decorations: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }



        
        
        yield return new WaitForSecondsRealtime(0.5f);

        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        RealTimeDayNightCycle.instance.SetDayTime(420, 1);
        PlayerDistanceToggle.instance.PopulateAnimalList();
        yield return new WaitForSecondsRealtime(3f);


        Time.timeScale = 1;
        UIScreenManager.instance.HideScreens(UIScreenType.LoadScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.StartScreen);

       
    }


    IEnumerator PlayerFadeInCo()
    {
        
        DissolveEffect.instance.StartDissolve(playerMaterial, 2f, true);
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerInformation.instance.playerShadow.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        
    }
}
