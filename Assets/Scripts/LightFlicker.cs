using System.Collections;
using UnityEngine;

public class LightFlicker : Lights
{

    
    private void Start()
    {
        Extinguish();
    }

    public override void Light()
    {
        
        StartCoroutine(Flicker());
    }

    public override void Extinguish()
    {
        base.Extinguish();
    }
    IEnumerator Flicker()
    {
        int flickerAmount = Random.Range(1, 5);
        int timesFlicked = 0;
        float timeBetweenFlickers = Random.Range(0.1f, 0.5f);
        lightsOn = true;

        while (timesFlicked <= flickerAmount)
        {
            lightObject.enabled = true;
            GameEventManager.onLightsToggleEvent.Invoke();

            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.1f, 0.5f);
            lightObject.enabled = false;
            GameEventManager.onLightsToggleEvent.Invoke();

            yield return new WaitForSeconds(timeBetweenFlickers);
            timeBetweenFlickers = Random.Range(0.1f, 0.7f);
            timesFlicked++;

            yield return null;

        }

        yield return new WaitForSeconds(timeBetweenFlickers);

        lightObject.enabled = true;
        GameEventManager.onLightsToggleEvent.Invoke();

        yield return null;

    }

    

   
}

