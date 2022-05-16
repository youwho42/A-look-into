using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{

    public Light2D light;
    public bool lightsOn;

    private void Start()
    {
        Exinguish();
    }
    public void LightAndFlicker()
    {
        StartCoroutine(Flicker());
    }

    public void Exinguish()
    {
        light.enabled = false;
        lightsOn = false;
    }
    IEnumerator Flicker()
    {
        int flickerAmount = Random.Range(1, 5);
        int timesFlicked = 0;
        float timeBetweenFlickers = Random.Range(0.1f, 0.5f);


        while (timesFlicked <= flickerAmount)
        {
            light.enabled = true;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.1f, 0.5f);
            light.enabled = false;
            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.1f, 0.7f);
            timesFlicked++;

            yield return null;

        }

        yield return new WaitForSeconds(timeBetweenFlickers);

        light.enabled = true;
        lightsOn = true;
        yield return null;

    }
}

