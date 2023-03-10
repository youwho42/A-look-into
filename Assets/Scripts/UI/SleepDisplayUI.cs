using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SleepDisplayUI : MonoBehaviour
{

    public static SleepDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    RealTimeDayNightCycle dayNightCycle;

    public GameObject sleepDisplay;
    PlayerInformation playerInformation;
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI sleepUntilTime;
    public Slider slider;
    private Coroutine sleepCoroutine;
    int originalCycleSpeed;
    bool isShowing;
    public void Start()
    {
        playerInformation = PlayerInformation.instance;
        dayNightCycle = RealTimeDayNightCycle.instance;
        originalCycleSpeed = dayNightCycle.cycleSpeed;
    }

    private void Update()
    {
        if (isShowing)
        {
            currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
            sleepUntilTime.text = string.Format("{0:00}:00", slider.value);
        }
        
    }
    public void ShowUI()
    {
        PlayerInformation.instance.uiScreenVisible = true;
        playerInformation.TogglePlayerInput(false);
        
        isShowing = true;
    }

    public void HideUI()
    {
        PlayerInformation.instance.uiScreenVisible = false;
        playerInformation.TogglePlayerInput(true);
        
        isShowing = false;
    }

    public void Sleep()
    {
        sleepCoroutine = StartCoroutine(SleepCo());
        
    }
    public void CancelSleep()
    {
        if(sleepCoroutine != null)
            StopCoroutine(sleepCoroutine);
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        playerInformation.TogglePlayerInput(true);
        sleepDisplay.SetActive(false);
    }
    IEnumerator SleepCo()
    {
        
        int wakeTime = (int)slider.value;
        
        while(dayNightCycle.hours != wakeTime)
        {
            //float currentEnergy = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce");
            PlayerInformation.instance.playerStats.AddToBounce(1f);
            dayNightCycle.cycleSpeed = 200;
            yield return null;
        }
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        HideUI();
        UIScreenManager.instance.HideScreens(UIScreenType.SleepScreen);
        UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        playerInformation.TogglePlayerInput(true);
        yield return null;

    }
}
