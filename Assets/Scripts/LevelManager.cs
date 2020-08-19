using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

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

    [SerializeField]
    private CinemachineVirtualCamera startCam;

    [SerializeField]
    private GameObject loadScreen;
    [SerializeField]
    private GameObject titleMenu;
    [SerializeField]
    private GameObject pauseMenu;




    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TextMeshProUGUI text;

    

    private void Start()
    {
       
        

    }

    

    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }
    public void StartNewGame(string levelName)
    {
        SavingLoading.instance.Save();
        titleMenu.SetActive(false);
    }
    public void LoadCurrentGame(string levelName)
    {
        LoadLevel(levelName);
    }
    public void SaveGame()
    {
        SavingLoading.instance.Save();
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
        pauseMenu.SetActive(isPaused);
    }

    IEnumerator ChangeLevelCo (string levelName)
    {
        AsyncOperation currentLevelLoading =  SceneManager.LoadSceneAsync(levelName);
        titleMenu.SetActive(false);
        loadScreen.SetActive(true);
        while (!currentLevelLoading.isDone)
        {
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            slider.value = progress;
            text.text = $"Loading: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }

        
            
       

        loadScreen.SetActive(false);


    }

    IEnumerator LoadLevelCo(string levelName)
    {
        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName);
        titleMenu.SetActive(false);
        loadScreen.SetActive(true);
        
        while (!currentLevelLoading.isDone)
        {
            
            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            slider.value = progress;
            text.text = $"Loading: {Mathf.RoundToInt(progress * 100)}%";

            
            yield return null;
        }
        
        
        SavingLoading.instance.Load();

        var cams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var cam in cams)
        {
            if (cam.CompareTag("StartCam"))
            {
                cam.Priority = 0;
            }
        }
        yield return new WaitForSeconds(3f);
        loadScreen.SetActive(false);
        FindObjectOfType<PlayerInput>().enabled = true;

    }


}
