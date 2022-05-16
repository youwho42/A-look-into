using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherGenerator : MonoBehaviour
{
    public GameObject weatherObject;
    RealTimeDayNightCycle realTimeDayNightCycle;

    private void Start()
    {
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(AcivateWeather);
        Invoke("Set", 2f);
    }

    void Set()
    {
        AcivateWeather(0);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(AcivateWeather);
    }

    void AcivateWeather(int time)
    {
        if (realTimeDayNightCycle.currentTimeRaw >= realTimeDayNightCycle.dayStart 
            && realTimeDayNightCycle.currentTimeRaw < realTimeDayNightCycle.dayStart + realTimeDayNightCycle.dayNightTransitionTime + 15 
            || realTimeDayNightCycle.currentTimeRaw >= realTimeDayNightCycle.nightStart 
            && realTimeDayNightCycle.currentTimeRaw < realTimeDayNightCycle.nightStart + realTimeDayNightCycle.dayNightTransitionTime + 15)
            weatherObject.SetActive(true);
        else
            weatherObject.SetActive(false);
    }
}
