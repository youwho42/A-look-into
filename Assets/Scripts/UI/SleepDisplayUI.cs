using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Klaxon.StatSystem;

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

    public bool isSleeping;
    public GameObject sleepDisplay;
    //PlayerInformation playerInformation;
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI sleepUntilTime;
    public Slider slider;
    private Coroutine sleepCoroutine;
    int originalCycleSpeed;
    bool isShowing;
    public StatChanger bounceStatChanger;
    public StatChanger gumptionStatChanger;
    public void Start()
    {
        //playerInformation = PlayerInformation.instance;
        dayNightCycle = RealTimeDayNightCycle.instance;
        //originalCycleSpeed = dayNightCycle.cycleSpeed;
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
        PlayerInformation.instance.TogglePlayerInput(false);
        UIScreenManager.instance.DisplayPlayerHUD(true);
        isShowing = true;
    }

    public void HideUI()
    {
        
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);
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
        dayNightCycle.cycleSpeed = 1;
        isSleeping = false;
        HideUI();
        sleepDisplay.SetActive(false);
    }
    IEnumerator SleepCo()
    {
        var player = PlayerInformation.instance;
        int wakeTime = (int)slider.value;
        
        while(dayNightCycle.hours != wakeTime)
        {
            isSleeping = true;
            //float currentEnergy = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce");
            player.statHandler.ChangeStat(bounceStatChanger);
            player.statHandler.ChangeStat(gumptionStatChanger);
            dayNightCycle.cycleSpeed = 0;
            yield return null;
        }
        dayNightCycle.cycleSpeed = 1;
        isSleeping = false;
        //UIScreenManager.instance.HideAllScreens();
        //UIScreenManager.instance.DisplayScreen(UIScreenType.PlayerUI);
        HideUI();
        yield return null;

    }
}
