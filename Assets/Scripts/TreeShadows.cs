using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeShadows : MonoBehaviour
{
    public Transform shadowTransform;
    public SpriteRenderer shadowSprite;

    DayNightCycle dayNightCycle;

    int shadowAppearTime = 5;
    int shadowDisappearTime = 20;

    bool shadowFade;
    
    private void Start()
    {
        dayNightCycle = DayNightCycle.instance;
        dayNightCycle.TickEventCallBack.AddListener(SetShadowRotation);
        dayNightCycle.FullHourEventCallBack.AddListener(StartShadowFade);
    }
    
    private void OnDisable()
    {
        dayNightCycle.TickEventCallBack.RemoveListener(SetShadowRotation);
        dayNightCycle.FullHourEventCallBack.RemoveListener(StartShadowFade);
    }

    public void StartShadowFade(int time)
    {
        if (time == shadowAppearTime)
            StartCoroutine("StartShadowsCo");
        if (time == shadowDisappearTime)
            StartCoroutine("EndShadowsCo");
    }

    IEnumerator StartShadowsCo()
    {
        
        float startTime = dayNightCycle.tick;
        float elapsedTime = 0;
        float waitTime = elapsedTime + 40;
        shadowSprite.transform.parent.gameObject.SetActive(true);
        float alpha = 0;
        while (elapsedTime < waitTime)
        {
            alpha = Mathf.Lerp(0, .5f, (elapsedTime / waitTime));
            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, alpha);
            elapsedTime = dayNightCycle.tick - startTime;

            yield return null;
        }

        shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, .5f);
        yield return null;
        
    }



    public void SetShadowRotation(int tick)
    {
        float elapsedTime = dayNightCycle.currentTimeRaw;
        float waitTime = shadowDisappearTime * 60;
        float zRotation = 0;

        zRotation = Mathf.Lerp(60, -60, (elapsedTime - 300) / (waitTime - 300));

        shadowTransform.eulerAngles = new Vector3(shadowTransform.eulerAngles.x, shadowTransform.eulerAngles.y, zRotation);

        
    }


    IEnumerator EndShadowsCo()
    {
        
        float startTime = dayNightCycle.tick;
        float elapsedTime = 0;
        float waitTime = elapsedTime + 40;
        float alpha = 0;

        while (elapsedTime < waitTime)
        {

            alpha = Mathf.Lerp(.5f, 0, (elapsedTime / waitTime));
            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, alpha);
            elapsedTime = dayNightCycle.tick - startTime;

            yield return null;
        }

        shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, 0);
        yield return null;
        shadowSprite.transform.parent.gameObject.SetActive(false);
        shadowTransform.eulerAngles = new Vector3(shadowTransform.eulerAngles.x, shadowTransform.eulerAngles.y, 0);
        
    }

}
