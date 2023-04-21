using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyFlicker : MonoBehaviour
{
    FireflyAI fireflyAI;
    //public SpriteRenderer fireflyLight;
    public SpriteRenderer firefly;
    Material fireflyMaterial;
    public float minBrightness = 1;
    public float maxBrightness = 60;
    bool flickering;
    Color initialColor;
    //float timeOut;
    //float timer;

    bool isOn;

    //[ColorUsageAttribute(true, true)]
    //public Color bright;
    //[ColorUsageAttribute(true, true)]
    //public Color dark;

    private void Start()
    {
        fireflyAI = GetComponent<FireflyAI>();
        fireflyMaterial = firefly.material;
        initialColor = fireflyMaterial.GetColor("_EmissionColor");
        fireflyMaterial.SetColor("_EmissionColor", initialColor * minBrightness);
        //timeOut = Random.Range(0.5f, 5.0f);
        isOn = false;
    }

    private void Update()
    {
        if (fireflyAI.currentState == FireflyAI.FlyingState.isFlying)
        {
            isOn = true;
            //timer += Time.deltaTime;
            if (!flickering)
            {
                //StopCoroutine("FlickerLight");
                StartCoroutine("Flicker");
            }

        }
        else
        {
            if (isOn)
            {
                //fireflyLight.color = new Color(fireflyLight.color.r, fireflyLight.color.g, fireflyLight.color.b, 0);
                fireflyMaterial.SetColor("_EmissionColor", initialColor*minBrightness);
                isOn = false;
            }
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

    //IEnumerator FlickerLight(float timeToFlick)
    //{
    //    float elapsedTime = 0;
    //    float waitTime = timeToFlick;
    //    float startLight = fireflyLight.color.a;
    //    float endLight = startLight > 0 ? 0 : 0.4f;
       
    //    Color startIntensity = startLight > 0 ? bright : dark;
    //    Color endIntensity = startLight > 0 ? dark : bright;
    //    while (elapsedTime < waitTime)
    //    {
    //        float a = Mathf.Lerp(startLight, endLight, elapsedTime / waitTime);
    //        Color b = Color.Lerp(startIntensity, endIntensity, elapsedTime / waitTime);
            
    //        fireflyLight.color = new Color(fireflyLight.color.r, fireflyLight.color.g, fireflyLight.color.b, a);
            
    //        fireflyMaterial.SetColor("_EmissionColor", b); 
            
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    fireflyLight.color = new Color(fireflyLight.color.r, fireflyLight.color.g, fireflyLight.color.b, endLight);
    //    fireflyMaterial.SetColor("_EmissionColor", endIntensity);
    //    yield return null;

    //}

}
