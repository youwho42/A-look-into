using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSway : MonoBehaviour
{
    Material material;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        
    }
    
    public void SwayItem()
    {
        StartCoroutine("SwayCo");
    }

    IEnumerator SwayCo()
    {
        float timePercentage = 0f;
        float fadeTime = 0.2f;
        material.SetVector("_WindMovement", new Vector4(Random.Range(1f, 2f), 0,0,0));
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(.01f, .1f, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }
        timePercentage = 0f;
        fadeTime = 3f;
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(.1f, .01f, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }
    }
}
