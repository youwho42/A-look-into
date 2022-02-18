using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Events;
using System;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

    

    public int tick;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        isDay = true;
        dayLength = 1440;
        dayStart = 300;
        nightStart = 1200;
        hours = Mathf.RoundToInt(currentTimeRaw / 60);
        minutes = currentTimeRaw % 60;
    }

    private int dayLength;      // in minutes
    private int dayStart;       // in minutes
    private int nightStart;     // in minutes
    [Range(0, 1440)]
    public int currentTimeRaw;  // in minutes
    public float cycleSpeed;
    private bool isDay;
    
    public Light2D sun;

    public int hours;
    public int minutes;
    public int currentDayRaw = 1;
    public bool fullHour;

    bool lightIsChanging;

    /*public FullHourEvent FullHourEventCallBack;
    public TickEvent TickEventCallBack;*/

    private Coroutine changeLightCoroutine;

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
        
        SetInitialLight(currentTimeRaw);
        /*if(currentTimeRaw >= nightStart + 20)
            sun.intensity = .3f;*/
        StartCoroutine(TimeOfDay());
        
    }

    public void PauseTimeCycle()
    {
        StopCoroutine("TimeOfDay");
    }

    public void UnPauseTimeCycle()
    {
        StartCoroutine(TimeOfDay());
    }

    void ChimeHour()
    {
        fullHour = true;
        //FullHourEventCallBack.Invoke(hours);
        GameEventManager.onTimeHourEvent.Invoke(hours);
    }

    void SetDayOrNight()
    {
        if (sun == null)
        {
            Light2D[] tempSun = FindObjectsOfType<Light2D>();
            for (int i = 0; i < tempSun.Length; i++)
            {
                if (!tempSun[i].CompareTag("Sun"))
                {
                    continue;
                }
                else
                {
                    sun = tempSun[i];
                    return;
                }
            }
        }
        
        switch (dayState)
        {
            case DayState.Sunrise:
                if(!lightIsChanging)
                    changeLightCoroutine = StartCoroutine(ChangeLight(1.2f));
                break;

            case DayState.Day:
                if (lightIsChanging)
                {
                    StopCoroutine(changeLightCoroutine);
                    lightIsChanging = false;
                }
                sun.intensity = 1.2f;
                break;

            case DayState.Sunset:
                if (!lightIsChanging)
                    changeLightCoroutine = StartCoroutine(ChangeLight(0.3f));
                break;

            case DayState.Night:
                if (lightIsChanging)
                {
                    StopCoroutine(changeLightCoroutine);
                    lightIsChanging = false;
                }
                    
                sun.intensity = 0.3f;
                break;
        }

        
    }

    void SetInitialLight(int currentTimeRaw)
    {
        if (currentTimeRaw >= 0 && currentTimeRaw < dayStart || currentTimeRaw > nightStart)
        {
            dayState = DayState.Night;
        }
        else if (currentTimeRaw > dayStart && currentTimeRaw < nightStart)
        {
            dayState = DayState.Day;
        }
        else if (currentTimeRaw == nightStart)
        {
            dayState = DayState.Sunset;
        }
        else if (currentTimeRaw == dayStart)
        {
            dayState = DayState.Sunrise;
        }
    }
    
    public void SetDayTime(int time, int day)
    {
        currentTimeRaw = time;
        currentDayRaw = day;
        SetInitialLight(currentTimeRaw);
    }

    // light changes in accordance with current time tick
    IEnumerator ChangeLight(float amount)
    {
        lightIsChanging = true;
        float elapsedTime = minutes;
        float waitTime = minutes + 40f;
        float intensity = sun.intensity;
        while (elapsedTime < waitTime)
        {
            sun.intensity = Mathf.Lerp(intensity, amount, (elapsedTime / waitTime));
            elapsedTime = minutes;

            yield return null;
        }
        sun.intensity = amount;
        dayState = amount == 1.2f ? DayState.Day : DayState.Night;
        lightIsChanging = false;
        yield return null;
    }


    // current time tick
    IEnumerator TimeOfDay()
    {
        while (true)
        {
            currentTimeRaw += 1;
            hours = Mathf.RoundToInt(currentTimeRaw / 60);
            minutes = currentTimeRaw % 60;
            if (minutes == 0 && fullHour == false)
            {
                ChimeHour();
                if (hours == 24)
                    currentDayRaw++;
            }
               
            if (minutes != 0)
                fullHour = false;
            if (currentTimeRaw >= dayLength)
                currentTimeRaw = 0;
            if(currentTimeRaw == dayStart)
                dayState = DayState.Sunrise;
            if (currentTimeRaw == nightStart)
                dayState = DayState.Sunset;
            SetDayOrNight();

            tick++;
            //TickEventCallBack.Invoke(tick);
            GameEventManager.onTimeTickEvent.Invoke(tick);

            yield return new WaitForSeconds(1f / cycleSpeed);
        }
    }

    /*[System.Serializable]
    public class FullHourEvent : UnityEvent<int> { }


    [System.Serializable]
    public class TickEvent : UnityEvent<int> { }
*/
}