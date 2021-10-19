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



    private void Start()
    {
        dayNightCycle = DayNightCycle.instance;
        dayNightCycle.FullHourEventCallBack.AddListener(StartShadows);
        LevelManager.instance.EventLevelLoaded.AddListener(ResetShadows);
        ResetShadows();
    }
    public void ResetShadows()
    {
        StopCoroutine("DayTimeShadows");
        StartCoroutine("DayTimeShadows");
    }
    public void StartShadows(int time)
    {
        if(time == shadowAppearTime)
        {
            shadowTransform.gameObject.SetActive(true);
            
            StartCoroutine("StartShadowsCo");
        }
    }
    IEnumerator StartShadowsCo()
    {
        StartCoroutine("DayTimeShadows");
        float startTime = dayNightCycle.tick;
        float elapsedTime = 0;
        float waitTime = elapsedTime + 40;
        /*float elapsedTime = dayNightCycle.minutes;
        float waitTime = dayNightCycle.minutes + 40f;*/
        
        while (elapsedTime < waitTime)
        {
            float alpha = Mathf.Lerp(0, .5f, (elapsedTime / waitTime));

            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, alpha);
            //elapsedTime = dayNightCycle.minutes;
            elapsedTime = dayNightCycle.tick - startTime;
            yield return null;
        }

        shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, .5f);
        yield return null;
    }

    IEnumerator DayTimeShadows()
    {


        float elapsedTime = dayNightCycle.currentTimeRaw;
        float waitTime = shadowDisappearTime * 60;
        
        
        while (elapsedTime < waitTime + 40)
        {
            float zRotation = Mathf.Lerp(60, -60, (elapsedTime - 300) / (waitTime - 300));
            
            shadowTransform.eulerAngles = new Vector3(shadowTransform.eulerAngles.x, shadowTransform.eulerAngles.y, zRotation);
            elapsedTime = dayNightCycle.currentTimeRaw;
            if(elapsedTime==waitTime)
                StartCoroutine("EndShadowsCo");
            yield return null;
        }
        
        
        
        yield return null;
    }

    IEnumerator EndShadowsCo()
    {

        float startTime = dayNightCycle.tick;
        float elapsedTime = 0;
        float waitTime = elapsedTime + 40;
        /*float elapsedTime = dayNightCycle.minutes;
        float waitTime = dayNightCycle.minutes + 40f;*/

        while (elapsedTime < waitTime)
        {
            float alpha = Mathf.Lerp(.5f, 0, (elapsedTime / waitTime));

            shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, alpha);
            //elapsedTime = dayNightCycle.minutes;
            elapsedTime = dayNightCycle.tick - startTime;

            yield return null;
        }

        shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, 0);
        yield return null;
        shadowTransform.gameObject.SetActive(false);
        shadowTransform.eulerAngles = new Vector3(shadowTransform.eulerAngles.x, shadowTransform.eulerAngles.y, 0);
    }
}
