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
    public Slider sleepSlider;
    private Coroutine sleepCoroutine;
    public Slider currentTimeSlider;

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

    public SleepClouds cloudObject;
    List<SleepClouds> cloudObjects = new List<SleepClouds>();
    int cloudCount = 10;
    public RectTransform cloudHolder;
    int lastSliderValue;
    int startTicks;
    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.SleepUI);
   
        gameObject.SetActive(false);
        dayNightCycle = RealTimeDayNightCycle.instance;
        CreateClouds();
    }

    void CreateClouds()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            var go = Instantiate(cloudObject, cloudHolder);
            go.ResetCloud();
            cloudObjects.Add(go);
            
        }
    }

    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(SetTime);
        GameEventManager.onTimeTickEvent.AddListener(SetSleepingDayNight);
        if(dayNightCycle != null)
        {
            sleepSlider.value = 180;
            
            SetTime(dayNightCycle.currentTimeRaw);
        }
        currentTimeSlider.value = 0;
        startTicks = 0;
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetTime);
        GameEventManager.onTimeTickEvent.RemoveListener(SetSleepingDayNight);
    }
    private void SetTime(int tick)
    {
        currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
       
    }
    public void SetSleepTime()
    {

        ConvertTicksToTime((int)sleepSlider.value);
        sleepUntilTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
        SetDayNightUI();
        //SetClouds();
        
        
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
        sleepSlider.interactable = true;
        HideUI();
        
    }
    IEnumerator SleepCo()
    {
        var player = PlayerInformation.instance;
        int wakeTime = (dayNightCycle.currentTimeRaw + (int)sleepSlider.value - 1) % 1440;
        sleepSlider.interactable = false;
        lastSliderValue = (int)sleepSlider.value;
        while (dayNightCycle.currentTimeRaw != wakeTime)
        {
            UIScreenManager.instance.isSleeping = true;
            player.statHandler.ChangeStat(bounceStatChanger);
            player.statHandler.ChangeStat(gumptionStatChanger);
            dayNightCycle.cycleSpeed = 0;
            yield return null;
        }
        dayNightCycle.cycleSpeed = 1;
        UIScreenManager.instance.isSleeping = false;
        sleepSlider.interactable = true;
        HideUI();
        yield return null;

    }
    void SetSleepingDayNight(int tick)
    {
       
        SetDayNightUI();
        SetCloudsSleep();
        if (UIScreenManager.instance.isSleeping)
        {
            startTicks++;
            if (startTicks > 10)
                SetCurrentTimeUI(startTicks);
            SetTimerText();
        }
            

            
    }
    void SetCurrentTimeUI(int tick)
    {
        currentTimeSlider.value = tick;
    }
    void SetTimerText()
    {
        lastSliderValue--;
        ConvertTicksToTime(lastSliderValue);
        sleepUntilTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    void SetDayNightUI()
    {
        int time = UIScreenManager.instance.isSleeping ? dayNightCycle.currentTimeRaw : (dayNightCycle.currentTimeRaw + (int)sleepSlider.value) % 1440;

        float starRot = MapNumber.Remap(time, 0, 1440, starObject.minRotation, starObject.maxRotation);
        starObject.transform.rotation = Quaternion.Euler(0, 0, starRot);

        float t = time >= dayNightCycle.dayStart && time <= dayNightCycle.nightStart ? time : 0;
        float sunRot = MapNumber.Remap(t, dayNightCycle.dayStart, dayNightCycle.nightStart, sunObject.minRotation, sunObject.maxRotation);
        sunObject.transform.rotation = Quaternion.Euler(0, 0, sunRot);

        float t2 = time >= dayNightCycle.nightStart || time <= dayNightCycle.dayStart ? time : 720;
        float moonRot = MapNumber.Remap((t2 + 500) % 1440, (dayNightCycle.nightStart+500) % 1440, (dayNightCycle.dayStart + 500), moonObject.minRotation, moonObject.maxRotation);
        moonObject.transform.rotation = Quaternion.Euler(0, 0, moonRot);

        if(time >= dayNightCycle.dayStart && time <= dayNightCycle.nightStart)
        {
            skyDayImage.color = new Color(1, 1, 1, 1);
            skyStarsImage.color = new Color(1, 1, 1, 0);
        } 
        else if (time >= dayNightCycle.nightStart || time <= dayNightCycle.dayStart)
        {
            skyDayImage.color = new Color(1, 1, 1, 0);
            skyStarsImage.color = new Color(1, 1, 1, 1);
        }
        if (time >= dayNightCycle.nightStart && time <= dayNightCycle.nightStart + dayNightCycle.dayNightTransitionTime)
            SetSky(time, true);
        else if (time >= dayNightCycle.dayStart && time <= dayNightCycle.dayStart + dayNightCycle.dayNightTransitionTime)
            SetSky(time, false);

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

    
    void SetCloudsSleep()
    {
       
        foreach (var cloud in cloudObjects)
        {
            cloud.CloudStep(-1);
        }
    }
}
