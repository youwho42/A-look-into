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

    public int hourOffset;
    public int minuteOffset;

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

    public Color dayColor;
    public Color nightColor;
    public Gradient[] sunsetColors;
    public Gradient[] sunriseColors;

    int gradientIndex;
    

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
        currentTimeRaw = ((now.Hour + hourOffset) * 60) + (now.Minute + minuteOffset);
        minutes = now.Minute + minuteOffset;
        hours = now.Hour + hourOffset;
        gradientIndex = Random.Range(0, sunsetColors.Length);
        SetDayState();
        
        StartCoroutine(TimeOfDay());
        Invoke("InitializeTick", 1.0f);
    }
    void InitializeTick()
    {
        GameEventManager.onTimeTickEvent.Invoke(currentTimeRaw);
    }
    
    

    void SetDayOrNightLight()
    {
        if (dayState == DayState.Sunrise)
            SetLight(false);
        if (dayState == DayState.Sunset)
            SetLight(true);
        if (dayState == DayState.Day)
            SetStableLight(true);
        if (dayState == DayState.Night)
            SetStableLight(false);

    }
    void SetStableLight(bool isDay)
    {
        gradientIndex = Random.Range(0, sunsetColors.Length);
        sun.intensity = isDay ? 1.2f : 0.3f; 
        sun.color = isDay ? dayColor : nightColor;
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
        float waitTime = dayNightTransitionTime;
        float intensity = dayToNight ? 1.2f : 0.3f;
        float amount = dayToNight ? 0.3f : 1.2f;
        sun.intensity = Mathf.Lerp(intensity, amount, elapsedTime / waitTime);
        sun.color = dayToNight ? sunsetColors[gradientIndex].Evaluate(elapsedTime / waitTime) : sunriseColors[gradientIndex].Evaluate(elapsedTime / waitTime);
    }
    

    

    // current time, ticking by
    IEnumerator TimeOfDay()
    {
        
        while (true)
        {
            var now = System.DateTime.Now;
            var m = now.Minute + minuteOffset;
            var h = now.Hour + hourOffset;
            currentTimeRaw = (h * 60) + m;


            if (now.Second == 0 && !minuteTicked)
            {
                minuteTicked = true;
                minutes = m;
                GameEventManager.onTimeTickEvent.Invoke(currentTimeRaw);

                SetDayState();
                
                
            }
            if (now.Second != 0 && minuteTicked)
                minuteTicked = false;

            if (m == 0 && !hourTicked)
            {
                hourTicked = true;
                hours = h;
                
                GameEventManager.onTimeHourEvent.Invoke(hours);
                
            }
            if (m != 0 && hourTicked)
                hourTicked = false;

            yield return null;
        }
    }
}
