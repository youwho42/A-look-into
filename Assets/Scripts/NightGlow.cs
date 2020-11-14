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
        dayNightCycle.FullHourEventCallBack.AddListener(StartGlow);
        material = spriteMaterial.material;
        initialIntensity = material.GetColor("_EmissionColor");
    }

    public void StartGlow(int time)
    {
        if (time == glowAppearTime)
            StartCoroutine(StartGlowCo(true));
        if (time == glowDisappearTime)
            StartCoroutine(StartGlowCo(false));
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
