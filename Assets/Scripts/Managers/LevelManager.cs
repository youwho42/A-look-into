using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using Klaxon.UndertakingSystem;
using Klaxon.SaveSystem;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);
        AllItemsDatabaseManager.instance.ResetItemsDatabase();
    }


    public bool loadTerrains;

    
    [SerializeField]
    private GameObject loadScreen;
    
    

    [SerializeField]
    private Slider loadScreenSlider;

    [SerializeField]
    private TextMeshProUGUI text;

    public bool isInCutscene;
    Material playerMaterial;
    [HideInInspector]
    public bool inPauseMenu;

    
    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }
    private IEnumerator Start()
    {
        if (Application.isPlaying && loadTerrains)
        {
            if (!SceneManager.GetSceneByName("MainScene-Grass").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Grass", LoadSceneMode.Additive);
            }

            if (!SceneManager.GetSceneByName("MainScene-Decoration").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Decoration", LoadSceneMode.Additive);
            }

            if (!SceneManager.GetSceneByName("MainScene-Flowers").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Flowers", LoadSceneMode.Additive);
            }

            if (!SceneManager.GetSceneByName("MainScene-Plants").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Plants", LoadSceneMode.Additive);
            }

            if (!SceneManager.GetSceneByName("MainScene-Trees").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Trees", LoadSceneMode.Additive);
            }

            if (!SceneManager.GetSceneByName("MainScene-Animals").isLoaded)
            {
                SceneManager.LoadSceneAsync("MainScene-Animals", LoadSceneMode.Additive);
            }

        }


        // create a tempsavefile of all the items from the latest version
        SavingLoading.instance.SaveVersionItems();


        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        isInCutscene = true;
        yield return new WaitForSeconds(0.2f);
        loadScreen.gameObject.SetActive(false);

        //AllItemsDatabaseManager.instance.ResetItemsDatabase();
        GameEventManager.onTimeHourEvent.Invoke(5);
#if UNITY_STANDALONE && !UNITY_EDITOR
        LoadTitleScreen();
#endif

    }




    public void PlayerSpriteAndNameAccepted()
    {
        isInCutscene = true;
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
        UIScreenManager.instance.inMainMenu = false;
        UIScreenManager.instance.PreventPlayerInputs(false);
        UIScreenManager.instance.DisplayPlayerHUD(UIScreenManager.instance.gameplay.HUDBinary == 1);
        GameEventManager.onPlayerPositionUpdateEvent.Invoke();
       
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    

    public void LoadCurrentGame(string levelName, string loadFileName)
    {
        LoadLevel(levelName, loadFileName);
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
        

    }

    

    IEnumerator LoadLevelCo(string levelName, string loadFileName)
    {
        var audioSettings = AudioSettingsUI.instance;
        GameEventManager.onGameStartLoadEvent.Invoke();
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.LoadScreenUI, true);
        UIScreenManager.instance.DisplayPlayerHUD(false);
        Pause(true);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();


        ////LOAD TERRAIN
        //AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName +"-Terrain");
        //audioSettings.Mute();
        //while (!currentLevelLoading.isDone)
        //{
            
        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);
            
        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";
            
            
        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD GRASS
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Grass", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Grass: {Mathf.RoundToInt(progress * 100)}%";
            
        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD FLOWERS
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Flowers", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Flowers: {Mathf.RoundToInt(progress * 100)}%";

        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD TREES
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Trees", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Trees: {Mathf.RoundToInt(progress * 100)}%";

        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD DECORATIONS
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Decorations: {Mathf.RoundToInt(progress * 100)}%";
           
        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD PLANTS
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Plants", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Plants: {Mathf.RoundToInt(progress * 100)}%";

        //    yield return null;
        //}
        //audioSettings.Mute();

        ////LOAD ANIMALS
        //currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Animals", LoadSceneMode.Additive);
        //while (!currentLevelLoading.isDone)
        //{

        //    float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

        //    loadScreenSlider.value = progress;
        //    text.text = $"Loading Animals: {Mathf.RoundToInt(progress * 100)}%";

        //    yield return null;
        //}
        //audioSettings.Mute();

        text.text = "Loading data from save.";
        // Something needs to be done about this. the scene is shown as loaded (because it is) at this point,
        // but the data still loads after this... figure it out?
        SavingLoading.instance.LoadGame(loadFileName);
        //ConsoleDebuggerUI.instance.SetDebuggerText($"{loadFileName} loaded");
        PlayerDistanceToggle.instance.PopulateLists();
        //ConsoleDebuggerUI.instance.SetDebuggerText("about to load version");
        SavingLoading.instance.LoadVersion();
        //ConsoleDebuggerUI.instance.SetDebuggerText("loaded version");
        if(!VersionDisplay.instance.CompareVersions())
            SavingLoading.instance.LoadVersionItems();

        
        yield return new WaitForSecondsRealtime(1.5f);

        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        
        GameEventManager.onGameLoadedEvent.Invoke();
       
        yield return new WaitForSecondsRealtime(2f);
        SavingLoading.instance.LoadOptions();

        
        //UIScreenManager.instance.DisplayPlayerHUD(HUDBinary == 1);
        
        GameEventManager.onStatUpdateEvent.Invoke();
        yield return new WaitForSecondsRealtime(0.1f);
        DissolveEffect.instance.StartDissolve(playerMaterial, 2f, true);
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerInformation.instance.playerShadow.SetActive(true); 
        yield return new WaitForSecondsRealtime(0.5f);
        Pause(false);
        ActivatePlayer();
        Time.timeScale = 1;
        UIScreenManager.instance.inMainMenu = false;
        UIScreenManager.instance.HideScreenUI();
    }

    IEnumerator LoadTitleScreenCo(string levelName)
    {
        var audioSettings = AudioSettingsUI.instance;
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.LoadScreenUI, true);
        UIScreenManager.instance.DisplayPlayerHUD(false);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();
        Pause(true);

        //LOAD TERRAIN
        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Terrain");
        audioSettings.Mute();
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }
        audioSettings.Mute();

        //LOAD GRASS
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Grass", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Grass: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        //LOAD FLOWERS
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Flowers", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Flowers: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        //LOAD TREES
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Trees", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Trees: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        //LOAD DECORATIONS
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Decorations: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        //LOAD PLANTS
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Plants", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Plants: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        //LOAD ANIMALS
        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Animals", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Animals: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }
        audioSettings.Mute();

        yield return new WaitForSecondsRealtime(1.0f);

        GameEventManager.onGameStartLoadEvent.Invoke();
        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        RealTimeDayNightCycle.instance.SetDayTime(420, 1);
        PlayerDistanceToggle.instance.PopulateLists();
        ResetAtDawnManager.instance.ResetAllItems(5);
        yield return new WaitForSecondsRealtime(2f);
        SavingLoading.instance.LoadOptions();
        Pause(false);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(3.0f);
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.inMainMenu = true;
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.MainMenuUI, true);
        
    }


    IEnumerator PlayerFadeInCo()
    {
        
        DissolveEffect.instance.StartDissolve(playerMaterial, 2f, true);
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerInformation.instance.playerShadow.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        
    }



    
}
