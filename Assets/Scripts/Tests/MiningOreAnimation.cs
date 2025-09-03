using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MiningOreAnimation : MonoBehaviour
{
    public List<Texture2D> emissionMaps = new List<Texture2D>();
    SpriteRenderer rend;
    MaterialPropertyBlock mpb;
    public int framerate = 16;
    float frameRefreshRate;
    int currentFrame;
    float animPauseLength;
    public float animationTime = 2.0f;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        frameRefreshRate = 1f / framerate;
        animPauseLength = animationTime - (frameRefreshRate * (emissionMaps.Count - 1));
        if(rend.isVisible)
            StartCoroutine("AnimateEmissionCo");
        else
            StopAllCoroutines();
    }
    private void OnBecameVisible()
    {
        StartCoroutine("AnimateEmissionCo");
    }

    private void OnBecameInvisible()
    {
        StopAllCoroutines();
    }

    IEnumerator AnimateEmissionCo()
    {
        rend.GetPropertyBlock(mpb);
        mpb.SetTexture("_Emission", emissionMaps[0]);
        rend.SetPropertyBlock(mpb);
        float pauseDeviation = animPauseLength * 0.2f;
        yield return new WaitForSeconds(Random.Range(0.0f, animPauseLength));
        bool started = true;
        //currentFrame = 1;
        while (true)
        {
            
            if(currentFrame == 0 && !started)
                yield return new WaitForSeconds(animPauseLength + Random.Range(-pauseDeviation, pauseDeviation));
            started = false;

            currentFrame = (currentFrame + 1) % emissionMaps.Count;

            rend.GetPropertyBlock(mpb);
            mpb.SetTexture("_Emission", emissionMaps[currentFrame]);
            rend.SetPropertyBlock(mpb);

            yield return new WaitForSeconds(frameRefreshRate);
        }
    }
}
