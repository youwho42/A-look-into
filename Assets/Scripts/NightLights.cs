using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLights : MonoBehaviour
{

    RealTimeDayNightCycle dayNightCycle;

    public Light2D[] lights;

    bool lightsOn;

    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetLights);
        foreach (var light in lights)
        {
            if (light.enabled)
                light.enabled = false;

        }
        lightsOn = false;
        Invoke("SetInitialLights", 1f);
    }

    void SetInitialLights()
    {
        SetLights(0);
    }
    
    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetLights);

    }
    public void SetLights(int time)
    {
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Night && !lightsOn)
        {
            
            foreach (var light in lights)
            {
                StartCoroutine(SetLight(light));
            }
            lightsOn = true;
        }
        else if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Day && lightsOn)
        {
            foreach (var light in lights)
            {
                light.enabled = false;
            }
            lightsOn = false;
        }

    }

    IEnumerator SetLight(Light2D light)
    {
        int flickerAmount = Random.Range(1, 8);
        int timesFlicked = 0;
        float timeBetweenFlickers = Random.Range(0.2f, .8f);
        

        while (timesFlicked <= flickerAmount) 
        {
            light.enabled = true;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.2f, .8f);
            light.enabled = false;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.2f, .8f);
            timesFlicked++;
                
            yield return null;
                
        }

        yield return new WaitForSeconds(timeBetweenFlickers);

        light.enabled = true;

        yield return null;

    }
}
