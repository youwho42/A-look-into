using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ContextSpeechBubbleHandler : MonoBehaviour
{

    public Transform speechBubbleTransform;
    public List<LocalizedString> localizedTexts = new List<LocalizedString>();
    public bool randomSpeechTimes;
    [ConditionalHide("randomSpeechTimes", true)]
    public Vector2Int minMaxTimeBetweenSpeak;
    CycleTicks nextSpeechCycle;
    RealTimeDayNightCycle timeCycle;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        timeCycle = RealTimeDayNightCycle.instance;
        if (randomSpeechTimes)
        {
            ResetNextRandomSpeechTime();
            GameEventManager.onTimeTickEvent.AddListener(SetRandomSpeechBubble);
        }
    }

    private void ResetNextRandomSpeechTime()
    {
        int nextTick = Random.Range(minMaxTimeBetweenSpeak.x, minMaxTimeBetweenSpeak.y);
        nextSpeechCycle = timeCycle.GetCycleTime(nextTick);
    }

    private void OnDisable()
    {
        if (randomSpeechTimes)
            GameEventManager.onTimeTickEvent.RemoveListener(SetRandomSpeechBubble);
        
    }

    void SetRandomSpeechBubble(int tick)
    {
        if (UIScreenManager.instance.isSleeping)
            return;
        if(tick >= nextSpeechCycle.tick && nextSpeechCycle.day >= timeCycle.currentDayRaw)
        {
            ResetNextRandomSpeechTime();
            int index = Random.Range(0, localizedTexts.Count);
            ContextSpeechBubbleManager.instance.SetContextBubble(3, speechBubbleTransform, localizedTexts[index].GetLocalizedString(), true);
        }
        

    }

    public void SetSpeechBubble(List<LocalizedString> localizedSpeechList)
    {
        if (UIScreenManager.instance.isSleeping)
            return;
        int index = Random.Range(0, localizedSpeechList.Count);
        ContextSpeechBubbleManager.instance.SetContextBubble(3, speechBubbleTransform, localizedSpeechList[index].GetLocalizedString(), true);
        
    }
}
