using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class PlantWindSway : MonoBehaviour
{
    public List<SpriteRenderer> swaySprites = new List<SpriteRenderer>();
    List<Material> swayMaterials = new List<Material>();
    [HideInInspector]
    public bool isVisible;
    public int windUpdateTick = 1;
    float maxWindStrength;
    float maxWindDensity;
    Vector2 currentDirection;
    Transform _transform;
    WindManager wind;

    bool materialsSet;

    private IEnumerator Start()
    {
        wind = WindManager.instance;
        _transform = transform;

        yield return new WaitForSeconds(1.0f);

        if (swaySprites[0].isVisible)
            OnBecameVisible();
        SetWind(windUpdateTick);

    }
    private void OnBecameVisible()
    {


        if (!materialsSet)
        {
            for (int i = 0; i < swaySprites.Count; i++)
            {
                swayMaterials.Add(swaySprites[i].material);
            }
            if (swayMaterials.Count > 0)
            {

                currentDirection = swayMaterials[0].GetVector("_WindMovement");
                maxWindStrength = swayMaterials[0].GetFloat("_WindStrength");
                maxWindDensity = swayMaterials[0].GetFloat("_WindDensity");

            }

            materialsSet = true;
        }


        InitializeSway();
        GameEventManager.onShadowTickEvent.AddListener(SetWind);
        isVisible = true;
        SetWind(windUpdateTick);

    }

    void InitializeSway()
    {
        var w = wind.GetWindMagnitudeNormalized(_transform.position);
        var d = wind.GetWindDirectionFromPosition(_transform.position);

        for (int i = 0; i < swayMaterials.Count; i++)
        {
            swayMaterials[i].SetVector("_WindMovement", d);
            swayMaterials[i].SetFloat("_WindStrength", maxWindStrength * w);
            swayMaterials[i].SetFloat("_WindDensity", maxWindDensity * w);
        }

    }

    private void OnBecameInvisible()
    {
        StopAllCoroutines();
        GameEventManager.onShadowTickEvent.RemoveListener(SetWind);
        isVisible = false;



    }
    private void OnDisable()
    {
        GameEventManager.onShadowTickEvent.RemoveListener(SetWind);

        StopAllCoroutines();
    }

    public void SetWind(int tick)
    {
        if (!isVisible)
            return;

        int sleep = 0;
        if (UIScreenManager.instance.isSleeping)
            sleep = 3;

        if (tick % (windUpdateTick + sleep) == 0)
            SetWindState();

    }

    void SetWindState()
    {
        StopAllCoroutines();
        StartCoroutine("ChangeForceCo");
    }

    IEnumerator ChangeForceCo()
    {
        var w = wind.GetWindMagnitudeNormalized(_transform.position);
        Vector2 lastDirection = swayMaterials[0].GetVector("_WindMovement");
        float lastStrength = swayMaterials[0].GetFloat("_WindStrength");
        float lastDensity = swayMaterials[0].GetFloat("_WindDensity");

        var nextDirection = wind.GetWindDirectionFromPosition(_transform.position);
        var finalStrength = maxWindStrength * w;
        float finalDensity = maxWindDensity * w;

        float t = 0;
        float maxT = windUpdateTick;

        while (t < maxT)
        {
            t += Time.deltaTime;

            var dir = Vector2.Lerp(lastDirection, nextDirection, t / maxT);
            float s = Mathf.Lerp(lastStrength, finalStrength, t / maxT);
            float d = Mathf.Lerp(lastDensity, finalDensity, t / maxT);
                

            foreach (var material in swayMaterials)
            {
                material.SetVector("_WindMovement", dir);
                material.SetFloat("_WindStrength", s);
                material.SetFloat("_WindDensity", d);
            }
            yield return null;
        }

        yield return null;
    }
}

