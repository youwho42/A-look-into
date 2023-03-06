using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDrop : MonoBehaviour
{
    SpriteRenderer render;
    
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }
    
    public void StartFade()
    {
        StartCoroutine(FadeCo());
    }

    IEnumerator FadeCo()
    {
        var mat = render.material;
        Color e = mat.GetColor("_EmissionColor");
        float timer = 0;
        float timeToFade = 4.5f;
        while (timer < timeToFade)
        {
            float a = Mathf.Lerp(1, 0, timer / timeToFade);
            float b = Mathf.Lerp(3, 0, timer / timeToFade);
            mat.SetColor("_EmissionColor", e * b);
            render.color = new Color(render.color.r, render.color.r, render.color.r, a);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
