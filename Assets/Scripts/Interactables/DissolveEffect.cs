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
        
        float timer = 0f;
        int from = dissovleIn ? 0 : 1;
        int to = dissovleIn ? 1 : 0;
        while (timer < dissolveTime)
        {
            timer += Time.deltaTime;
            float x = Mathf.Lerp(from, to, timer/dissolveTime);
            material.SetFloat("_Fade", x);
            
            yield return null;
        }
    }
}
