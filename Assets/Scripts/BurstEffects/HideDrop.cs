using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDrop : MonoBehaviour
{
    SpriteRenderer render;
    ItemFuzzGlow glow;
    FadeDropOnHeight fadeDrop;
    PurpleFireSheet fireSheet;
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        glow = GetComponent<ItemFuzzGlow>();
        fadeDrop = GetComponent<FadeDropOnHeight>();
        fireSheet = GetComponentInParent<PurpleFireSheet>();
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
        float timeToFade = 5f;
        fadeDrop.enabled = true;
        float maxHeight = fireSheet.maxHeight;
        while (timer < timeToFade)
        {
            float a = Mathf.Lerp(maxHeight, 1.0f, timer / timeToFade);
            fireSheet.maxHeight = a;
            float b = Mathf.Lerp(3, 0, timer / timeToFade);
            mat.SetColor("_EmissionColor", e * b);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
