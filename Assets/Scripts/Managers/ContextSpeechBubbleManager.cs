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


    public ContextSpeechBubble speechBubbleObject;
    public ObjectPool<ContextSpeechBubble> speechBubblePool;
    int poolAmount = 3;

    public void Start()
    {

        speechBubblePool = new ObjectPool<ContextSpeechBubble>
            (
                createFunc: CreateSmell,
                actionOnGet: GetFromPool,
                actionOnRelease: ReleaseToPool,
                defaultCapacity: poolAmount

            );

    }
    public ContextSpeechBubble SetContextBubble(float time, Transform _transform, string text, bool useLine)
    {
        var bubble = speechBubblePool.Get();
        bubble.transform.position = _transform.position;
        bubble.SetText(time, text, _transform, useLine);
        return bubble;
    }
    public void CloseSpeechBubble(ContextSpeechBubble speechBubble)
    {
        if (speechBubble == null)
            return;
        
        speechBubblePool.Release(speechBubble);
    }

    ContextSpeechBubble CreateSmell()
    {
        ContextSpeechBubble speechBubble = Instantiate(speechBubbleObject, transform);
        speechBubble.transform.position = Vector3.zero;
        speechBubble.gameObject.SetActive(true);
        
        return speechBubble;
    }

    void GetFromPool(ContextSpeechBubble speechBubble)
    {
        speechBubble.gameObject.SetActive(true);
    }

    void ReleaseToPool(ContextSpeechBubble speechBubble)
    {
        speechBubble.gameObject.SetActive(false);
    }

}
