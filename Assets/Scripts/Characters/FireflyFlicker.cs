using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyFlicker : MonoBehaviour
{
    FireflyAI fireflyAI;
    public SpriteRenderer fireflyLight;
    public SpriteRenderer firefly;
    Material fireflyMaterial;
    float timeOut;
    float timer;
    private void Start()
    {
        fireflyAI = GetComponent<FireflyAI>();
        fireflyMaterial = firefly.material;
        timeOut = Random.Range(0.5f, 5.0f);
        
    }

    private void Update()
    {
        if(fireflyAI.currentState == FireflyAI.FlyingState.isFlying)
        {
            timer += Time.deltaTime;
            if (timer >= timeOut)
            {
                StopCoroutine("FlickerLight");
                StartCoroutine(FlickerLight(Random.Range(0.1f, 2.0f)));
                timer = 0;
                timeOut = Random.Range(0.1f, 5.0f);
            }

        }

    }

    IEnumerator FlickerLight(float timeToFlick)
    {
        float elapsedTime = 0;
        float waitTime = timeToFlick;
        float startLight = fireflyLight.color.a;
        float endLight = startLight > 0 ? 0 : 0.4f;
        Color initialColor = fireflyMaterial.GetColor("_EmissionColor");
        float startIntensity = startLight > 0 ? 1 : -1;
        float endIntensity = startLight > 0 ? -1 : 1;
        while (elapsedTime < waitTime)
        {
            float a = Mathf.Lerp(startLight, endLight, elapsedTime / waitTime);
            float b = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / waitTime);
            
            fireflyLight.color = new Color(fireflyLight.color.r, fireflyLight.color.g, fireflyLight.color.b, a);
            if(b != 0)
                fireflyMaterial.SetColor("_EmissionColor", initialColor * b); 
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fireflyLight.color = new Color(fireflyLight.color.r, fireflyLight.color.g, fireflyLight.color.b, endLight);
        fireflyMaterial.SetColor("_EmissionColor", initialColor * endIntensity);
        yield return null;

    }

}
