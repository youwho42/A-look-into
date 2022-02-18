using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightGlow : MonoBehaviour
{
    DayNightCycle dayNightCycle;

    public int glowAppearTime;
    public int glowDisappearTime;
    public float bloomMinIntensity;
    public float bloomMaxIntensity;

    public SpriteRenderer spriteMaterial;
    Material material;
    Color initialIntensity;

    private void Start()
    {
        dayNightCycle = DayNightCycle.instance;
        GameEventManager.onTimeHourEvent.AddListener(StartGlow);
        material = spriteMaterial.material;
        initialIntensity = material.GetColor("_EmissionColor");
        SetInitialGlow();
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(StartGlow);
    }
    public void StartGlow(int time)
    {
        if (time == glowAppearTime)
            StartCoroutine(StartGlowCo(true));
        if (time == glowDisappearTime)
            StartCoroutine(StartGlowCo(false));
    }
    
    void SetInitialGlow()
    {
        if(dayNightCycle.hours>=5 || dayNightCycle.hours <= 20)
        {
            material.SetColor("_EmissionColor", initialIntensity * bloomMinIntensity);
        }
        else
        {
            material.SetColor("_EmissionColor", initialIntensity * bloomMaxIntensity);
        }
    }

    IEnumerator StartGlowCo(bool on) 
    {
        
        float elapsedTime = dayNightCycle.minutes;
        float waitTime = elapsedTime + 40f;
        
        float startIntensity = on ? bloomMinIntensity : bloomMaxIntensity;
        float endIntinsity = on ? bloomMaxIntensity : bloomMinIntensity;
        
        
        while (elapsedTime < waitTime)
        {
            float i = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));

            material.SetColor("_EmissionColor", initialIntensity * i);
            elapsedTime = dayNightCycle.minutes;

            yield return null;
        }

        material.SetColor("_EmissionColor", initialIntensity * endIntinsity);
        yield return null;
    }
}
