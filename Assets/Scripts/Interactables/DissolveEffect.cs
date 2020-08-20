using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{

    public static DissolveEffect instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void StartDissolve(Material material, float dissolveTime, bool dissovleIn)
    {
        StartCoroutine(StartDissolveCo(material, dissolveTime, dissovleIn));
    }


    IEnumerator StartDissolveCo(Material material, float dissolveTime, bool dissovleIn)
    {
        float timePercentage = 0f;
        float fadeTime = 2f;
        int from = dissovleIn ? 0 : 1;
        int to = dissovleIn ? 1 : 0;
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(from, to, timePercentage);
            material.SetFloat("_Fade", x);

            yield return null;
        }
    }
}
