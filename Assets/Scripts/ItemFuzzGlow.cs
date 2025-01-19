using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFuzzGlow : MonoBehaviour
{

    SpriteRenderer fuzzGlowImage;
    public float minGlow, maxGlow;
    public float bloomMinIntensity;
    public float bloomMaxIntensity;
    RealTimeDayNightCycle dayNightCycle;
    Material material;
    Color initialColor;
    public bool useAlpha, useIntensity;
    bool isStable;
    
    private void Start()
    {
        
        dayNightCycle = RealTimeDayNightCycle.instance;
        fuzzGlowImage = GetComponent<SpriteRenderer>();
        material = fuzzGlowImage.material;
        initialColor = material.GetColor("_EmissionColor");
        
    }
    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(SetGlow);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetGlow);
        isStable = false;
    }

    public void SetGlow(int time)
    {
        if(dayNightCycle == null)
            dayNightCycle = RealTimeDayNightCycle.instance;
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunrise)
            SetGlowAmount(time, false);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunset)
            SetGlowAmount(time, true);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Day && !isStable)
            SetStableGlow(true);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Night && !isStable)
            SetStableGlow(false);
        
    }
    void SetStableGlow(bool state)
    {
        if (useAlpha)
        {
            float alpha = state ? minGlow : maxGlow;
            Color c = new Color(fuzzGlowImage.color.r, fuzzGlowImage.color.g, fuzzGlowImage.color.b, alpha);
            fuzzGlowImage.color = c;
        }
        if (useIntensity)
        {
            float intensity = state ? bloomMinIntensity : bloomMaxIntensity;
            if(material != null)
                material.SetColor("_EmissionColor", initialColor * intensity);
        }
        isStable = true;
        
    }
    void SetGlowAmount(int time, bool state)
    {
        isStable = false;
        float elapsedTime = time - ( state ? dayNightCycle.nightStart : dayNightCycle.dayStart);
        //elapsedTime = Mathf.Clamp(elapsedTime, 0, 1440);
        float waitTime = dayNightCycle.dayNightTransitionTime;
       
        if (useIntensity)
        {
            float startIntensity = state ? bloomMinIntensity : bloomMaxIntensity;
            float endIntinsity = state ? bloomMaxIntensity : bloomMinIntensity;
            float j = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));
            if(material != null)
                material.SetColor("_EmissionColor", initialColor * j);
        }

        if (useAlpha)
        {
            float intensity = state ? minGlow : maxGlow;
            float amount = state ? maxGlow : minGlow;
            float i = Mathf.Lerp(intensity, amount, elapsedTime / waitTime);
            Color c = new Color(fuzzGlowImage.color.r, fuzzGlowImage.color.g, fuzzGlowImage.color.b, i);
            fuzzGlowImage.color = c;
        }
       
    }

    
}
