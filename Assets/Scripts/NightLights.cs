using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLights : MonoBehaviour
{

    RealTimeDayNightCycle dayNightCycle;

    public LightFlicker[] lights;



    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetLights);
       
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
