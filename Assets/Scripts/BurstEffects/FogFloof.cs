using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFloof : MonoBehaviour
{

    public SpriteRenderer sprite;
    public Gradient fogCycle;
    RealTimeDayNightCycle realTimeDayNightCycle;
    bool isVisible;
    private IEnumerator Start()
    {
        
        yield return new WaitForSeconds(1f);
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetFloofs);
        Invoke("Set", 2f);
    }

    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetFloofs);
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
    void Set()
    {
        
        SetFloofs(0);
    }
    public void SetFloofs(int time)
    {
        
        if (realTimeDayNightCycle.currentTimeRaw >= realTimeDayNightCycle.dayStart && realTimeDayNightCycle.currentTimeRaw < realTimeDayNightCycle.dayStart + realTimeDayNightCycle.dayNightTransitionTime + 15)
            SetFloofVisibility(false);
        else if (realTimeDayNightCycle.currentTimeRaw >= realTimeDayNightCycle.nightStart && realTimeDayNightCycle.currentTimeRaw < realTimeDayNightCycle.nightStart + realTimeDayNightCycle.dayNightTransitionTime + 15)
            SetFloofVisibility(true);
        else
            sprite.color = fogCycle.Evaluate(1);
    }


    void SetFloofVisibility(bool dayToNight)
    {
        float elapsedTime = realTimeDayNightCycle.currentTimeRaw - (dayToNight ? realTimeDayNightCycle.nightStart : realTimeDayNightCycle.dayStart);
        float waitTime = realTimeDayNightCycle.dayNightTransitionTime + 15;
        
        if (sprite != null)
            sprite.color = fogCycle.Evaluate(elapsedTime / waitTime);

    }


}
