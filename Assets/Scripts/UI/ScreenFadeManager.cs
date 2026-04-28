using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


public class ScreenFadeManager : MonoBehaviour
{
    public static ScreenFadeManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public Image imageFade;
    private void Start()
    {
        imageFade.gameObject.SetActive(false);
        Color c = imageFade.color;
        c.a = 0.0f;
        imageFade.color = c;
    }

    public void FadeScreen(float time, bool fadeIn)
    {
        StartCoroutine(FadeScreenCo(time, fadeIn));
    }
    IEnumerator FadeScreenCo(float time, bool fadeIn)
    {
        
        Color c = imageFade.color;
        c.a = fadeIn ? 1.0f : 0.0f;
        imageFade.color = c;
        imageFade.gameObject.SetActive(true);
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            
            float a = Mathf.Lerp(1.0f, 0.0f, timer/time);
            if (!fadeIn)
                a = Mathf.Abs(a - 1.0f);
            c.a = a;
            imageFade.color = c;
            yield return null;
        }

        if (!fadeIn)
        {
            StartCoroutine(FadeScreenCo(time * .5f, true));
        }
        yield return null;
        imageFade.gameObject.SetActive(false);
    }
}
