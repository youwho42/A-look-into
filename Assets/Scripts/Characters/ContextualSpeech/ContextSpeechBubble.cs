using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContextSpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI speechText;
    public GameObject lineObject;
    public GameObject thoughtObject;

    public void SetText(float time, string text, Transform _transform, bool useLine)
    {
        speechText.text = text;
        StartCoroutine(SetSpeechBubbleCo(time, _transform));
        lineObject.SetActive(false);
        thoughtObject.SetActive(false);
        if (useLine)
            lineObject.SetActive(true);
        else
            thoughtObject.SetActive(true);
    }

    IEnumerator SetSpeechBubbleCo(float time, Transform _transform)
    {

        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            if (_transform != null)
                transform.position = _transform.position;
            yield return null;
        }
        
        ContextSpeechBubbleManager.instance.CloseSpeechBubble(this);
    }

}
