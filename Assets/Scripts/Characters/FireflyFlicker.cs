using Klaxon.SAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyFlicker : MonoBehaviour
{
    public SpriteRenderer firefly;
    Material fireflyMaterial;
    public float minBrightness;
    public float maxBrightness;
    bool flickering;
    Color initialColor;
    bool isOn;
    SAP_WorldBeliefStates worldState;

    private void Start()
    {
        worldState = SAP_WorldBeliefStates.instance;
        fireflyMaterial = firefly.material;
        initialColor = fireflyMaterial.GetColor("_EmissionColor");
        fireflyMaterial.SetColor("_EmissionColor", initialColor * minBrightness);
        isOn = false;

        
    }
    private void OnEnable()
    {
        InvokeRepeating("Flick", 1f, 1f);
        
    }

    private void OnDisable()
    {
        CancelInvoke("Flick");
    }

    void Flick()
    {
        
        if (worldState.HasWorldState("AnimalDay", false))
        {
            isOn = true;
            if (!flickering)
            {
                StartCoroutine(Flicker());
            }
        }
        else if (isOn)
        {
            fireflyMaterial.SetColor("_EmissionColor", initialColor * minBrightness);
            isOn = false;
            flickering = false;
        }

    }

    IEnumerator Flicker()
    {
        flickering = true;

        // fade in
        float elapsedTime = 0;
        float waitTime = Random.Range(0.1f, 1.0f);
        while (elapsedTime < waitTime)
        {
            float j = Mathf.Lerp(minBrightness, maxBrightness, (elapsedTime / waitTime));
            fireflyMaterial.SetColor("_EmissionColor", initialColor * j);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // stay lit
        elapsedTime = 0;
        waitTime = Random.Range(0.5f, 3.0f);
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // fade out
        elapsedTime = 0;
        waitTime = Random.Range(0.1f, 1.0f);
        while (elapsedTime < waitTime)
        {
            float j = Mathf.Lerp(maxBrightness, minBrightness, (elapsedTime / waitTime));
            fireflyMaterial.SetColor("_EmissionColor", initialColor * j);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // stay off
        elapsedTime = 0;
        waitTime = Random.Range(0.5f, 3.0f);
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flickering = false;
    }



}
