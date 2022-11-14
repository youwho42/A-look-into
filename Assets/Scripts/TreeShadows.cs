using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;

    RealTimeDayNightCycle realTimeDayNightCycle;
    
    bool isVisible = true;

    private void Start()
    {
        
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        
    }
    private void OnBecameVisible()
    {
        isVisible = true;
        SetShadows(0);
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetShadows);
    }

    public void SetShadows(int time)
    {
        if (!isVisible)
            return;
        SetShadowRotation();
        StartShadowFade();
    }

    public void StartShadowFade()
    {
        if (shadowSprite == null)
            return;
        if (realTimeDayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunrise)
            SetShadowVisibility(false);
        else if (realTimeDayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunset)
            SetShadowVisibility(true);
        else if (realTimeDayNightCycle.dayState == RealTimeDayNightCycle.DayState.Day)
            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, 0.5f);
        else
            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, 0.0f);
        
        shadowSprite.enabled = realTimeDayNightCycle.dayState != RealTimeDayNightCycle.DayState.Night;
    }


    void SetShadowVisibility(bool dayToNight)
    {
        float elapsedTime = realTimeDayNightCycle.currentTimeRaw - (dayToNight ? realTimeDayNightCycle.nightStart : realTimeDayNightCycle.dayStart);
        float waitTime = realTimeDayNightCycle.dayNightTransitionTime;
        float alpha = dayToNight ? 0.5f : 0.0f;
        float amount = dayToNight ? 0.0f : 0.5f;
        alpha = Mathf.Lerp(alpha, amount, elapsedTime / waitTime);
        if (shadowSprite != null)
            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, alpha);

    }


    public void SetShadowRotation()
    {
        

        float elapsedTime = realTimeDayNightCycle.currentTimeRaw - realTimeDayNightCycle.dayStart;
        float waitTime = (realTimeDayNightCycle.nightStart + realTimeDayNightCycle.dayNightTransitionTime) - realTimeDayNightCycle.dayStart;
        float zRotation = 0;
        
        zRotation = Mathf.Lerp(80, -80, elapsedTime / waitTime);
        float shadowLength = Mathf.Lerp(0.4f,1.4f, Mathf.Abs(zRotation)/80);
        shadowTransform.eulerAngles = new Vector3(shadowTransform.eulerAngles.x, shadowTransform.eulerAngles.y, zRotation);
        shadowSprite.transform.localScale = new Vector3(1, shadowLength, 1);
    }


}
