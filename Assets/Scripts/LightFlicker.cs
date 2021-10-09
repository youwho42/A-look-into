using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public Light2D lightToAffect;

    float startFalloff;
    float startIntensity;

    public bool useFalloff;
    public bool useIntensity;
    [Range(0,1)]
    public float flickerAmount;
    public float waitTimeAmount;
    bool isFlickering;
    private void Start()
    {
        startIntensity = lightToAffect.intensity;
        startFalloff = lightToAffect.falloffIntensity;
    }
    public void StartLightFlicker()
    {
        StartCoroutine(FadeInAndOutRepeat());
    }
    IEnumerator FadeInAndOut()
    {

        isFlickering = true;
        
        float elapsedTime = 0;
        float waitTime = Random.Range(0.01f, 0.2f);
        float intensity = lightToAffect.intensity;
        float tempIntensity = startIntensity + Random.Range(-flickerAmount, flickerAmount);

        while (elapsedTime < waitTime)
        {

            lightToAffect.intensity = Mathf.Lerp(intensity, tempIntensity, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        lightToAffect.intensity = tempIntensity;

        yield return null;
        isFlickering = false;
    }

    IEnumerator FadeInAndOutRepeat()
    {
        

        while (true)
        {
            
            if (!isFlickering)
                StartCoroutine(FadeInAndOut());

            
            yield return null;

            
        }
    }

}

