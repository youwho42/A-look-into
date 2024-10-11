using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGenerator : MonoBehaviour
{
    public GameObject fogObject;
    public float spawnBounds;
    public List<Transform> fogPositions = new List<Transform>();
    RealTimeDayNightCycle realTimeDayNightCycle;
    CycleTicks nextCycle;
    int nextCycleDuration;
    bool isActive;
    bool fading;
    bool fadeIn;
    int fadeTick = 40;
    int fadeAmount;
    private void Start()
    {
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(AcivateWeather);
        SetNextCycle();
        Invoke("Set", 1f);
    }

    private void SetNextCycle()
    {
        int r = Random.Range(420, 2880);
        //r = 5;
        nextCycle = realTimeDayNightCycle.GetCycleTime(r);
        nextCycleDuration = Random.Range(69, 420);
    }

    void Set()
    {
        fogObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(AcivateWeather);
    }

    void AcivateWeather(int time)
    {

        
        if (nextCycle.day == realTimeDayNightCycle.currentDayRaw && nextCycle.tick == time && !isActive)
        {
            fadeAmount = 0;
            fading = true;
            fadeIn = true;
            isActive = true;
            SetPosition();
            fogObject.SetActive(true);
        }
        if (isActive)
        {
            nextCycleDuration--;
            if (nextCycleDuration <= fadeTick)
            {
                fadeAmount = fadeTick;
                fading = true;
                fadeIn = false;
                isActive = false;
            }    
        }
        if (fading)
        {
            fadeAmount = fadeIn ? fadeAmount+=1 : fadeAmount-=1;
            float a = (float)fadeAmount / (float)fadeTick;
            float amount = NumberFunctions.RemapNumber(a, 0.0f, 1.0f, 0.0f, 0.1f);
            if (fogObject.TryGetComponent(out IWeatherObject weather))
                weather.Activate(amount);
            if (fadeIn && a >= 1.0f || !fadeIn && a <= 0.0f)
            {
                fading = false;
                fogObject.SetActive(fadeIn);
                SetNextCycle();

            }
        }

    }

    private void SetPosition()
    {
        int p = Random.Range(0, fogPositions.Count);
        Vector3 pos = new Vector3(Random.Range(0, spawnBounds), Random.Range(0, spawnBounds/2), 0);
        fogObject.transform.position = fogPositions[p].position + pos;
    }

    private void OnDrawGizmosSelected()
    {
        
        foreach (var pos in fogPositions)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(pos.position, new Vector3(spawnBounds, spawnBounds / 2, 0));
           
        }
    }
}
