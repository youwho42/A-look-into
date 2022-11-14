using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightGlow : MonoBehaviour
{
    RealTimeDayNightCycle dayNightCycle;

    
    public float bloomMinIntensity;
    public float bloomMaxIntensity;

    public SpriteRenderer spriteMaterial;
    Material material;
    Color initialColor;

    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(StartGlow);
        material = spriteMaterial.material;
        initialColor = material.GetColor("_EmissionColor");
        SetInitialGlow();
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(StartGlow);
    }
    public void StartGlow(int time)
    {
        if (time >= dayNightCycle.nightStart || time < dayNightCycle.dayStart)
            StartGlowCo(true);
        if (time <= dayNightCycle.dayStart || time > dayNightCycle.nightStart)
            StartGlowCo(false);
    }

    void SetInitialGlow()
    {
        if (dayNightCycle.hours >= 5 || dayNightCycle.hours <= 20)
        {
            material.SetColor("_EmissionColor", initialColor * bloomMinIntensity);
        }
        else
        {
            material.SetColor("_EmissionColor", initialColor * bloomMaxIntensity);
        }
    }

    void StartGlowCo(bool on) 
    {
        
        float elapsedTime = dayNightCycle.minutes;
        float waitTime = elapsedTime + 40f;
        
        float startIntensity = on ? bloomMinIntensity : bloomMaxIntensity;
        float endIntinsity = on ? bloomMaxIntensity : bloomMinIntensity;

        float i = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));
        
        material.SetColor("_EmissionColor", initialColor * i);
        //elapsedTime = dayNightCycle.minutes;

        //while (elapsedTime < waitTime)
        //{
        //    float i = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));

        //    material.SetColor("_EmissionColor", initialIntensity * i);
        //    elapsedTime = dayNightCycle.minutes;

            
        //}

        
        
    }
}
