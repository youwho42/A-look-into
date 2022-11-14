using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using UnityEngine.Rendering.Universal;

public class FireFlicker : MonoBehaviour
{
    public Light2D lightToAffect;
    private static FieldInfo m_FalloffField = typeof(Light2D).GetField("m_FalloffIntensity", BindingFlags.NonPublic | BindingFlags.Instance);

    float startFalloff;
    float startIntensity;

    
    [Range(0, 1)]
    public float flickerAmount;
    public float waitTimeAmount;
    bool isFlickering;
    private void Start()
    {
        startIntensity = lightToAffect.intensity;
        startFalloff = lightToAffect.falloffIntensity;
        StartLightFlicker();
    }

    public void SetFalloff(float falloff)
    {
        m_FalloffField.SetValue(lightToAffect, falloff);

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
            SetFalloff(lightToAffect.intensity * .4f);
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