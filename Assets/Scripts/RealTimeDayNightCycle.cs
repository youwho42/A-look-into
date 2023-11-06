using System.Collections;
using UnityEngine;

public class CycleTicks
{
    public int tick;
    public int day;
}

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

    private readonly WaitForEndOfFrame waitForFixedUpdate = new();

    public float deltaTick;
    float lastTick;

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

    float timer;
    
    bool resetGradient;
    bool gradientSet;
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
        SetGradient();
    }

    public CycleTicks GetCycleTime(int ticks)
    {
        var cycle = new CycleTicks();
        int time = currentTimeRaw;
        int d = ticks / 1440;
        cycle.day = currentDayRaw + d;
        int t = ticks % 1440;
        cycle.tick = time + t;
        return cycle;
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

    void SetGradient()
    {
        gradientIndex = Random.Range(0, sunsetColors.Length);
        gradientSet = true;
    }
    
    void SetStableLight(bool isDay)
    {
        
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
        if(intensity == amount)
        {
            gradientSet = false;
        }
    }

    // current time, ticking by
    IEnumerator TimeOfDay()
    {
        while (true)
        {
            float currentTickTime = Time.time;
            deltaTick = currentTickTime - lastTick;
            //deltaTick *= 0.02f;
            //deltaTick = MapNumber.Remap(deltaTick, 0.0f, 1.0f, 0.1f, 0.02f);
            
            lastTick = currentTickTime;
            if (!isPaused)
            {

                currentTimeRaw++;
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

                // Setting new sunset/sunrise colors
                if (hours % 12 == 0)
                    SetGradient();
            }
            if (cycleSpeed == 1)
                yield return new WaitForSeconds(1f * PlayerInformation.instance.statHandler.GetStatMaxModifiedValue("TimeDilation"));
            else
                yield return new WaitForEndOfFrame();

        }

    }

}
