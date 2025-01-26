using System.Collections;
using UnityEngine;

public class SquonkPuddle : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float puddleLifeTime = 10.0f;
    [Range(0.0f, 1.0f)]
    public float fadeStartPercent;

    Color fadingColor = Color.white;
    private void OnEnable()
    {
        StartCoroutine(FadePuddle());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadePuddle()
    {
        fadingColor.a = 1;
        sprite.color = fadingColor;
        float waitTime = puddleLifeTime * fadeStartPercent;
        yield return new WaitForSeconds(waitTime);
        float fadeTime = puddleLifeTime * (1.0f - fadeStartPercent);
        float timer = 0;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1.0f, 0.0f, timer / fadeTime);
            fadingColor.a = a;
            sprite.color = fadingColor;
            yield return null;
        }
        yield return null;
        gameObject.SetActive(false);
        
    }
}
