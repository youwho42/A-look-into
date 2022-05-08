using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FadeBlack : MonoBehaviour
{
    
    public Image image;
    public TextMeshProUGUI[] title;
    private void Start()
    {
        Invoke("StartFade", 5);
    }
    void StartFade()
    {
        
        
        StartCoroutine(Fade(5));
    }

    IEnumerator Fade(float time)
    {
        image.enabled = true;
        float elapsedTime = 0;
        float waitTime = time;
        float startLight = image.color.a;
        float endLight = startLight > 0 ? 0 : 1f;
        
        while (elapsedTime < waitTime)
        {
            float a = Mathf.Lerp(startLight, endLight, elapsedTime / waitTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            foreach (var text in title)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, a);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach (var text in title)
        {
            text.enabled = false;
        }
        image.enabled = false;
       

        yield return null;
        
    }
}
