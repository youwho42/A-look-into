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
    
    public void SwaySoft()
    {
        StartCoroutine(SwayCo(.2f, 1, .05f));
    }

    public void SwayMedium()
    {
        StartCoroutine(SwayCo(1, 2, .1f));
    }

    IEnumerator SwayCo(float swayMin, float swayMax, float strength)
    {
        float timePercentage = 0f;
        float fadeTime = 0.2f;
        material.SetVector("_WindMovement", new Vector4(Random.Range(swayMin, swayMax), 0,0,0));
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(.01f, strength, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }
        timePercentage = 0f;
        fadeTime = 3f;
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(strength, .01f, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }
    }

    
}
