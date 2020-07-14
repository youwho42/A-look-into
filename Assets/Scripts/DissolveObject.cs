using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObject : MonoBehaviour
{

    [SerializeField]
    private Material material;

   public float dissolveSpeed;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Dissolve();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DissolveReverse();
        }
    }

    public void Dissolve()
    {
        StartCoroutine("DissolveCo");
    }

    public void DissolveReverse()
    {
        StartCoroutine("DissolveReverseCo");
    }

    IEnumerator DissolveCo()
    {
        float timePercentage = 0f;
        float amount = 1;
        while (timePercentage < 1)
        {
            timePercentage += Time.deltaTime / dissolveSpeed;
            float x = Mathf.Lerp(amount, 0, timePercentage);
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