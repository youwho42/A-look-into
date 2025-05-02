using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;

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
    bool shadowCasterEnabled = false;

    //[HideInInspector]
    public List<Light2D> allLights = new List<Light2D>();

    private IEnumerator Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;

        GameEventManager.onTimeTickEvent.AddListener(SetShadows);
        GameEventManager.onPlayerPlacedItemEvent.AddListener(GetAllLights);
        yield return new WaitForSeconds(0.5f);
        GetAllLights();
        SetShadows(0);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetShadows);
        GameEventManager.onPlayerPlacedItemEvent.RemoveListener(GetAllLights);

    }

    void GetAllLights()
    {
        allLights = FindObjectsByType<Light2D>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = allLights.Count-1; i >= 0; i--)
        {
            if (allLights[i].lightType == Light2D.LightType.Global)
                allLights.Remove(allLights[i]);
        }
    }
    public Light2D GetClosestLightSource(Vector3 position)
    {
        Light2D closest = null;
        float nearest = float.MaxValue;
        for (int i = 0; i < allLights.Count; i++)
        {
            if (!allLights[i].isActiveAndEnabled || !allLights[i].gameObject.isStatic)
                continue;
            var dist = ((Vector2)allLights[i].transform.position - (Vector2)position).sqrMagnitude;
            if (dist <= allLights[i].pointLightOuterRadius && dist < nearest)
            {
                nearest = dist;
                closest = allLights[i];
            }
        }
        return closest;
    }

    public Color GetShadowColor()
    {
        return shadowColor;
    }
    public bool GetShadowVisible()
    {
        return dayNightCycle.dayState != RealTimeDayNightCycle.DayState.Night;
    }
    public bool ShadowCasterEnabled()
    {
        return shadowCasterEnabled;
    }
    

    public void SetShadows(int time)
    {
        SetShadowRotation();
        StartShadowFade();
        
        GameEventManager.onShadowTickEvent.Invoke(time);
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

        shadowCasterEnabled = shadowColor.a <= 0.2f;
        


    }


    void SetShadowVisibility(bool dayToNight)
    {
        float elapsedTime = RealTimeDayNightCycle.instance.currentTimeRaw - (dayToNight ? RealTimeDayNightCycle.instance.nightStart : RealTimeDayNightCycle.instance.dayStart);
        float waitTime = RealTimeDayNightCycle.instance.dayNightTransitionTime;
        float alpha = dayToNight ? 0.5f : 0.0f;
        float amount = dayToNight ? 0.0f : 0.5f;
        alpha = Mathf.Lerp(alpha, amount, elapsedTime / waitTime);
        //shadowColor = new Color(shadowColor.r, shadowColor.g, shadowColor.b, alpha);
        shadowColor = new Color(0, 0, 0, alpha);
        
    }


    public void SetShadowRotation()
    {
        float elapsedTime = RealTimeDayNightCycle.instance.currentTimeRaw - RealTimeDayNightCycle.instance.dayStart;
        float waitTime = (RealTimeDayNightCycle.instance.nightStart + RealTimeDayNightCycle.instance.dayNightTransitionTime) - RealTimeDayNightCycle.instance.dayStart;
        float zRotation = 0;

        zRotation = Mathf.Lerp(80, -80, elapsedTime / waitTime);
        float shadowLength = Mathf.Lerp(0.5f, 1.4f, Mathf.Abs(zRotation) / 80);
        shadowRotation = new Vector3(0, 0, zRotation);
        shadowScale = new Vector3(1, shadowLength, 1);
        //if(zRotation >= 58.5f && zRotation <=62.5f)
        //    Debug.Log($"{zRotation}");
    }

}
