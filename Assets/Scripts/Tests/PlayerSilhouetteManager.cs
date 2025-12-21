using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


public class PlayerSilhouetteManager : MonoBehaviour
{
    public static PlayerSilhouetteManager instance;
    public bool isBehindCliff;
    
    
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public RawImage image;
    public float maxAlpha = 0.45f;

    public void SetColor(float alpha)
    {
        StopAllCoroutines();

        float a = UIScreenManager.instance.GetCurrentUI() == UIScreenType.MiniGameUI ? 0 : maxAlpha * alpha;


        image.color = new Color(1, 1, 1, a);
    }

    public void SetColor(float alpha, float time, bool isBehindCliff)
    {
        
        StopAllCoroutines();
        float a = UIScreenManager.instance.GetCurrentUI() == UIScreenType.MiniGameUI ? 0 : maxAlpha * alpha;
        StartCoroutine(SetColorCo(a, time, isBehindCliff));

    }

    IEnumerator SetColorCo(float alpha, float time, bool isBehindCliff)
    {
        
        
        this.isBehindCliff = isBehindCliff;
        
        float maxTime = time;
        float currentAlpha = image.color.a;
        time = 0;
        while (time <= maxTime)
        {
            time += Time.deltaTime;
            var a = Mathf.Lerp(currentAlpha, alpha, time / maxTime);
            image.color = new Color(1, 1, 1, a);
            yield return null;
        }
        image.color = new Color(1, 1, 1, alpha);
        
       
        
        
    }
}
