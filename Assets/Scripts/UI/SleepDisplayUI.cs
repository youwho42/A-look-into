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
    int hours;
    int minutes;
    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.SleepUI);
   
        gameObject.SetActive(false);
        dayNightCycle = RealTimeDayNightCycle.instance;
    }

    private void OnEnable()
    {
        slider.value = (dayNightCycle.currentTimeRaw + 60) % 1440;
    }

    private void Update()
    {
        if (isShowing)
        {
            currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
            ConvertTicksToTime((int)slider.value);
            sleepUntilTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
        
    }

    void ConvertTicksToTime(int currentTimeRaw)
    {
        hours = Mathf.RoundToInt(currentTimeRaw / 60);
        minutes = currentTimeRaw % 60;
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

        UIScreenManager.instance.HideScreenUI();
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
        UIScreenManager.instance.isSleeping = false;
        HideUI();
        
    }
    IEnumerator SleepCo()
    {
        var player = PlayerInformation.instance;
        int wakeTime = (int)slider.value;
        
        while(dayNightCycle.currentTimeRaw != wakeTime)
        {
            UIScreenManager.instance.isSleeping = true;
            player.statHandler.ChangeStat(bounceStatChanger);
            player.statHandler.ChangeStat(gumptionStatChanger);
            dayNightCycle.cycleSpeed = 0;
            yield return null;
        }
        dayNightCycle.cycleSpeed = 1;
        UIScreenManager.instance.isSleeping = false;
        
        HideUI();
        yield return null;

    }
}
