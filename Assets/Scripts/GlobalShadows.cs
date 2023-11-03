using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShadows : MonoBehaviour
{
    public static GlobalShadows instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    RealTimeDayNightCycle dayNightCycle;

    public Vector3 shadowRotation { get; private set; }
    public Vector3 shadowScale { get; private set; }
    
    Color shadowColor = Color.black;

    private IEnumerator Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;

        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        yield return new WaitForSeconds(0.5f);
        
        SetShadows(0);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetShadows);
    }

    public Color GetShadowColor()
    {
        return shadowColor;
    }
    public bool GetShadowVisible()
    {
        return dayNightCycle.dayState != RealTimeDayNightCycle.DayState.Night;
    }

    

    public void SetShadows(int time)
    {
        SetShadowRotation();
        StartShadowFade();
    }

    public void StartShadowFade()
    {
        
        if (RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Sunrise)
            SetShadowVisibility(false);
        else if (RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Sunset)
            SetShadowVisibility(true);
        else if (RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Day)
            shadowColor = new Color(shadowColor.r, shadowColor.g, shadowColor.b, 0.5f);
        else
            shadowColor = new Color(shadowColor.r, shadowColor.g, shadowColor.b, 0.0f);

        
    }


    void SetShadowVisibility(bool dayToNight)
    {
        float elapsedTime = RealTimeDayNightCycle.instance.currentTimeRaw - (dayToNight ? RealTimeDayNightCycle.instance.nightStart : RealTimeDayNightCycle.instance.dayStart);
        float waitTime = RealTimeDayNightCycle.instance.dayNightTransitionTime;
        float alpha = dayToNight ? 0.5f : 0.0f;
        float amount = dayToNight ? 0.0f : 0.5f;
        alpha = Mathf.Lerp(alpha, amount, elapsedTime / waitTime);
        shadowColor = new Color(shadowColor.r, shadowColor.g, shadowColor.b, alpha);
        
    }


    public void SetShadowRotation()
    {
        float elapsedTime = RealTimeDayNightCycle.instance.currentTimeRaw - RealTimeDayNightCycle.instance.dayStart;
        float waitTime = (RealTimeDayNightCycle.instance.nightStart + RealTimeDayNightCycle.instance.dayNightTransitionTime) - RealTimeDayNightCycle.instance.dayStart;
        float zRotation = 0;

        zRotation = Mathf.Lerp(80, -80, elapsedTime / waitTime);
        float shadowLength = Mathf.Lerp(0.4f, 1.4f, Mathf.Abs(zRotation) / 80);
        shadowRotation = new Vector3(0, 0, zRotation);
        shadowScale = new Vector3(1, shadowLength, 1);
    }

}
