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
    public int currentTimeRaw;     // in minutes
    public int currentDayRaw = 1;
    public int cycleSpeed;
    public bool isPaused;

    UnityEngine.Rendering.Universal.Light2D sun;

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
        isPaused = true;
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent< UnityEngine.Rendering.Universal.Light2D>();
        hours = Mathf.RoundToInt(currentTimeRaw / 60);
        minutes = currentTimeRaw % 60;
        gradientIndex = Random.Range(0, sunsetColors.Length);
        SetDayState();
        
        StartCoroutine(TimeOfDay());
        Invoke("InitializeTick", 1.0f);
    }
    void InitializeTick()
    {
        GameEventManager.onTimeTickEvent.Invoke(currentTimeRaw);
    }

    public void SetDayTime(int time, int day)
    {
        if(sun == null)
            sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        currentTimeRaw = time;
        currentDayRaw = day;
        SetDayState();
        InitializeTick();
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
            if (!isPaused) {
                
                currentTimeRaw += 1;
                if (currentTimeRaw == 1440)
                {
                    currentDayRaw++;
                    currentTimeRaw = 0;
                }
                hours = Mathf.RoundToInt(currentTimeRaw / 60);
                minutes = currentTimeRaw % 60;

                if (minutes == 0 && !hourTicked)
                {
                    hourTicked = true;
                    GameEventManager.onTimeHourEvent.Invoke(hours);
                }
                if (minutes != 0 && hourTicked)
                    hourTicked = false;

                SetDayState();
                GameEventManager.onTimeTickEvent.Invoke(currentTimeRaw);
                    
                
            }
            yield return new WaitForSeconds(1f / cycleSpeed);
        }
        
    }


}
