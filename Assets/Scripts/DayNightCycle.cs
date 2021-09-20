using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

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

    public FullHourEvent FullHourEventCallBack;

    // Day and Night Script for 2d,
    // Unity needs one empty GameObject (earth) and one Light (sun)
    
    void Start()
    {
        
        if(currentTimeRaw >= nightStart + 20)
            sun.intensity = .3f;
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
        FullHourEventCallBack.Invoke(hours);
        /*if (hours == 5)
            AudioManager.instance.PlaySound("Rooster");*/
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
            
        if (currentTimeRaw >= 0 && currentTimeRaw < dayStart)
        {
            if (isDay)
            {
                isDay = false;
                sun.intensity = .3f;
            }
        }
        else if (currentTimeRaw >= dayStart && currentTimeRaw < nightStart)
        {
            if (!isDay)
            {
                isDay = true;
                StartCoroutine(ChangeLight(1.2f));
                
            }
        }
        else if (currentTimeRaw >= nightStart && currentTimeRaw < dayLength)
        {
            if (isDay)
            {
                isDay = false;
                StartCoroutine(ChangeLight(0.3f));
            }
        }
        else if (currentTimeRaw >= dayLength)
        {
            currentTimeRaw = 0;
        }
        
    }

    
    public void SetDayTime(int time, int day)
    {
        currentTimeRaw = time;
        currentDayRaw = day;
    }

    // light changes in accordance with current time tick
    IEnumerator ChangeLight(float amount)
    {
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

            SetDayOrNight();

            yield return new WaitForSeconds(1F / cycleSpeed);
        }
    }

    [System.Serializable]
    public class FullHourEvent : UnityEvent<int> { }

}