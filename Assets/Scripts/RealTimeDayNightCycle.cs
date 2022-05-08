using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeDayNightCycle : MonoBehaviour
{
    public static RealTimeDayNightCycle instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
  
    }

    public int dayStart = 300;     // in minutes
    public int nightStart = 1200;     // in minutes
    [Range(0, 1440)]
    public int currentTimeRaw;  // in minutes
    

    public UnityEngine.Rendering.Universal.Light2D sun;

    public int hours;
    public int minutes;
    
    public int dayNightTransitionTime = 15;

    bool hourTicked;
    bool minuteTicked;

    // Day and Night Script for 2d,
    // Unity needs one empty GameObject (earth) and one Light (sun)

    public DayState dayState;
    public enum DayState
    {
        Sunrise,
        Day,
        Sunset,
        Night
    }

    void Start()
    {
        var now = System.DateTime.Now;
        currentTimeRaw = (now.Hour * 60) + now.Minute;
        minutes = now.Minute;
        hours = now.Hour;
        SetDayState();

        StartCoroutine(TimeOfDay());
        Invoke("InitializeTick", 1.0f);
    }
    void InitializeTick()
    {
        GameEventManager.onTimeTickEvent.Invoke(0);
    }
    
    

    void SetDayOrNightLight()
    {
        if (dayState == DayState.Sunrise)
            SetLight(false);
        if (dayState == DayState.Sunset)
            SetLight(true);
        if (dayState == DayState.Day)
            sun.intensity = 1.2f;
        if (dayState == DayState.Night)
            sun.intensity = 0.3f;

    }

    void SetDayState()
    {
        if (currentTimeRaw >= 0 && currentTimeRaw < dayStart || currentTimeRaw > nightStart + dayNightTransitionTime)
            dayState = DayState.Night;
        else if (currentTimeRaw > dayStart + dayNightTransitionTime && currentTimeRaw < nightStart)
            dayState = DayState.Day;
        else if (currentTimeRaw >= nightStart && currentTimeRaw <= nightStart + dayNightTransitionTime)
            dayState = DayState.Sunset;
        else if (currentTimeRaw >= dayStart && currentTimeRaw <= dayStart + dayNightTransitionTime)
            dayState = DayState.Sunrise;
        SetDayOrNightLight();
    }

    void SetLight(bool dayToNight)
    {
        
        float elapsedTime = currentTimeRaw - (dayToNight ? nightStart : dayStart);
        float waitTime =  dayNightTransitionTime;
        float intensity = dayToNight ? 1.2f : 0.3f;
        float amount = dayToNight ? 0.3f : 1.2f;
        sun.intensity = Mathf.Lerp(intensity, amount, elapsedTime / waitTime);

    }
    

    

    // current time tick
    IEnumerator TimeOfDay()
    {
        
        while (true)
        {
            var now = System.DateTime.Now;
            currentTimeRaw = (now.Hour * 60) + now.Minute;


            if (now.Second == 0 && !minuteTicked)
            {
                minuteTicked = true;
                minutes = now.Minute;
                GameEventManager.onTimeTickEvent.Invoke(now.Second);

                SetDayState();
                
                
            }
            if (now.Second != 0 && minuteTicked)
                minuteTicked = false;

            if (now.Minute == 0 && !hourTicked)
            {
                hourTicked = true;
                hours = now.Hour;
                
                GameEventManager.onTimeHourEvent.Invoke(now.Hour);
                
            }
            if (now.Minute != 0 && hourTicked)
                hourTicked = false;

            yield return null;
        }
    }
}
