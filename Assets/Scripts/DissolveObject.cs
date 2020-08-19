using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObject : MonoBehaviour
{

    public Material material;

    public float dissolveSpeed;

    
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        Dissolve();
    //    }
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        DissolveReverse();
    //    }
    //}

    public void Dissolve()
    {
        StartCoroutine(DissolveCo(true));
    }

    public void DissolveReverse()
    {
        StartCoroutine(DissolveCo(false));
    }

    IEnumerator DissolveCo(bool disolveOut = true)
    {
        float timePercentage = 0f;
        float disolveFrom = disolveOut ? 0 : 1;
        float disolveTo = disolveOut ? 1 : 0;

        while (timePercentage < 1)
        {
            timePercentage += Time.deltaTime / dissolveSpeed;
            float x = Mathf.Lerp(disolveFrom, disolveTo, timePercentage);
            material.SetFloat("_DissolveAmount", x);
            material.SetFloat("_Offset", x*20);
            yield return null;
        }

    }

    IEnumerator DissolveReverseCo()
    {
        float timePercentage = 0f;
        float amount = 0;
        while (timePercentage < 1)
        {
            timePercentage += Time.deltaTime / dissolveSpeed;
            float x = Mathf.Lerp(amount, 1, timePercentage);
            material.SetFloat("_DissolveAmount", x);
            material.SetFloat("_Offset", x*10);
            yield return null;
        }

    }

}