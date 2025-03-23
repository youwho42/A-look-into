using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class ContextSpeechBubbleManager : MonoBehaviour
{
    public static ContextSpeechBubbleManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public ObjectPooler speechBubblePool;
    
    public ContextSpeechBubble SetContextBubble(float time, Transform _transform, string text, bool useLine)
    {
        var go = speechBubblePool.GetPooledObject();
        var bubble = go.GetComponent<ContextSpeechBubble>();
        bubble.transform.position = _transform.position;
        bubble.SetText(time, text, _transform, useLine);
        return bubble;
    }
    public void CloseSpeechBubble(ContextSpeechBubble speechBubble)
    {
        if (speechBubble == null)
            return;

        speechBubble.gameObject.SetActive(false);
    }

    
}
