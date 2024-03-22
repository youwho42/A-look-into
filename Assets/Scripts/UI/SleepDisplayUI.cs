using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Klaxon.StatSystem;
using System;

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
    
    public StatChanger bounceStatChanger;
    public StatChanger gumptionStatChanger;
    int hours;
    int minutes;
    UIScreen screen;

    [Serializable]
    public struct DayNightObject
    {
        public RectTransform transform;
        public Image image;
        public float minRotation;
        public float maxRotation;
    }

    public DayNightObject starObject;
    public DayNightObject sunObject;
    public DayNightObject moonObject;
    public Image skyDayImage;
    public Image skyStarsImage;
    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.SleepUI);
   
        gameObject.SetActive(false);
        dayNightCycle = RealTimeDayNightCycle.instance;
    }

    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(SetTime);
        GameEventManager.onTimeTickEvent.AddListener(SetSleepingDayNight);
        if(dayNightCycle != null)
            slider.value = (dayNightCycle.currentTimeRaw + 60) % 1440;
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetTime);
        GameEventManager.onTimeTickEvent.RemoveListener(SetSleepingDayNight);
    }
    private void SetTime(int tick)
    {
            currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
            //if(UIScreenManager.instance.isSleeping == true)
            //    SetDayNightUI();
    }
    public void SetSleepTime()
    {
        ConvertTicksToTime((int)slider.value);
        sleepUntilTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
        SetDayNightUI((int)slider.value);
    }
    void ConvertTicksToTime(int currentTimeRaw)
    {
        hours = Mathf.RoundToInt(currentTimeRaw / 60);
        minutes = currentTimeRaw % 60;
    }

    public void ShowUI()
    {
        UIScreenManager.instance.DisplayPlayerHUD(true);
       
    }

    public void HideUI()
    {

        UIScreenManager.instance.HideScreenUI();
        
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
    void SetSleepingDayNight(int tick)
    {
        if(UIScreenManager.instance.isSleeping)
            SetDayNightUI(tick);
    }


    void SetDayNightUI(int currentTime)
    {
        
        float starRot = MapNumber.Remap(currentTime, 0, 1440, starObject.minRotation, starObject.maxRotation);
        starObject.transform.rotation = Quaternion.Euler(0, 0, starRot);

        float t = currentTime >= dayNightCycle.dayStart && currentTime <= dayNightCycle.nightStart ? currentTime : 0;
        float sunRot = MapNumber.Remap(t, dayNightCycle.dayStart, dayNightCycle.nightStart, sunObject.minRotation, sunObject.maxRotation);
        sunObject.transform.rotation = Quaternion.Euler(0, 0, sunRot);

        float t2 = currentTime >= dayNightCycle.nightStart || currentTime <= dayNightCycle.dayStart ? currentTime : 720;
        float moonRot = MapNumber.Remap((t2 + 500) % 1440, (dayNightCycle.nightStart+500) % 1440, (dayNightCycle.dayStart + 500), moonObject.minRotation, moonObject.maxRotation);
        moonObject.transform.rotation = Quaternion.Euler(0, 0, moonRot);

        if(currentTime >= dayNightCycle.dayStart && currentTime <= dayNightCycle.nightStart)
        {
            skyDayImage.color = new Color(1, 1, 1, 1);
            skyStarsImage.color = new Color(1, 1, 1, 0);
        } 
        else if (currentTime >= dayNightCycle.nightStart || currentTime <= dayNightCycle.dayStart)
        {
            skyDayImage.color = new Color(1, 1, 1, 0);
            skyStarsImage.color = new Color(1, 1, 1, 1);
        }
        if (currentTime >= dayNightCycle.nightStart && currentTime <= dayNightCycle.nightStart + dayNightCycle.dayNightTransitionTime)
            SetSky(currentTime, true);
        else if (currentTime >= dayNightCycle.dayStart && currentTime <= dayNightCycle.dayStart + dayNightCycle.dayNightTransitionTime)
            SetSky(currentTime, false);

    }

    void SetSky(int currentTime, bool dayToNight)
    {
        float elapsedTime = currentTime - (dayToNight ? dayNightCycle.nightStart : dayNightCycle.dayStart);
        float waitTime = dayNightCycle.dayNightTransitionTime;
        float startAlpha = dayToNight ? 1f : 0.0f;
        float endAlpha = dayToNight ? 0.0f : 1f;

        float skyAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / waitTime);
        skyDayImage.color = new Color(1, 1, 1, skyAlpha);

        float starAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / waitTime);
        skyStarsImage.color = new Color(1, 1, 1, starAlpha);

    }
}
