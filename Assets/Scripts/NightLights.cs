using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLights : MonoBehaviour
{

    RealTimeDayNightCycle dayNightCycle;

    public List<LightFlicker> lights = new List<LightFlicker>();



    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetLights);
        lights = GetComponentsInChildren<LightFlicker>().ToList();
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
        
        if (dayNightCycle.currentTimeRaw >= dayNightCycle.nightStart + (dayNightCycle.dayNightTransitionTime - 10) || dayNightCycle.currentTimeRaw < dayNightCycle.dayStart)
        {

            foreach (var light in lights)
            {
                if(!light.lightsOn)
                    light.LightAndFlicker();
            }

        }
        else if (dayNightCycle.currentTimeRaw >= dayNightCycle.dayStart + 10 && dayNightCycle.currentTimeRaw < dayNightCycle.nightStart)
        {
            foreach (var light in lights)
            {
                if (light.lightsOn)
                    light.Exinguish();
            }
        }

    }

    
}
