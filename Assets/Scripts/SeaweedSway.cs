using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaweedSway : MonoBehaviour
{
    Material material;
    public float swayTimeIn;
    public float swayTimeOut;


    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.position.z == transform.position.z)
            StartCoroutine("SwayCo");
    }



    IEnumerator SwayCo()
    {

        float timePercentage = 0f;
        float fadeTime = swayTimeIn;
        
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(0.1f, 0.4f, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }
        timePercentage = 0f;
        fadeTime = swayTimeOut;
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(0.4f, 0.1f, timePercentage);
            material.SetFloat("_WindStrength", x);

            yield return null;
        }

    }
    void OnBecameInvisible()
    {
        enabled = false;
    }
    void OnBecameVisible()
    {
        enabled = true;
    }
}
