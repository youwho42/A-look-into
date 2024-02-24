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
            
        }


        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        isInCutscene = true;
        yield return new WaitForSeconds(0.2f);
        loadScreen.gameObject.SetActive(false);

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
        GameEventManager.onGameStartLoadEvent.Invoke();
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.LoadScreenUI, true);
        UIScreenManager.instance.DisplayPlayerHUD(false);
        Pause(true);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();

        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName +"-Terrain");

        while (!currentLevelLoading.isDone)
        {
            
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }

        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Grass", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Grass: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }

        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Trees and Animals: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }


        text.text = "Loading data from save.";
        // Something needs to be done about this. the scene is shown as loaded (because it is) at this point,
        // but the data still loads after this... figure it out?
        SavingLoading.instance.LoadGame(loadFileName);
        PlayerDistanceToggle.instance.PopulateLists();

        yield return new WaitForSecondsRealtime(0.5f);

        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        
        GameEventManager.onGameLoadedEvent.Invoke();
        
        yield return new WaitForSecondsRealtime(3f);
       

        Time.timeScale = 1;
        UIScreenManager.instance.inMainMenu = false;
        UIScreenManager.instance.HideScreenUI();
        //UIScreenManager.instance.DisplayPlayerHUD(HUDBinary == 1);
        
        GameEventManager.onStatUpdateEvent.Invoke();
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
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.LoadScreenUI, true);
        UIScreenManager.instance.DisplayPlayerHUD(false);
        Klaxon_C_U_DatabaseHolder.instance.undertakingDatabase.ResetUndertakings();
        Pause(true);
        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Terrain");

        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Terrain: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }

        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Grass", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Grass: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }

        currentLevelLoading = SceneManager.LoadSceneAsync(levelName + "-Decoration", LoadSceneMode.Additive);
        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            text.text = $"Loading Trees and Animals: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }




        yield return new WaitForSecondsRealtime(0.5f);

        text.text = "Thank you for waiting.";
        playerMaterial = PlayerInformation.instance.playerSprite.material;
        playerMaterial.SetFloat("_Fade", 0);
        PlayerInformation.instance.playerShadow.SetActive(false);
        RealTimeDayNightCycle.instance.SetDayTime(420, 1);
        PlayerDistanceToggle.instance.PopulateLists();
        yield return new WaitForSecondsRealtime(3f);
        Pause(false);
        Time.timeScale = 1;
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
