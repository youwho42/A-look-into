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
    private void OnEnable()
    {
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
        if(tick >= nextSpeechCycle.tick && nextSpeechCycle.day == timeCycle.currentDayRaw)
        {
            ResetNextRandomSpeechTime();
            int index = Random.Range(0, localizedTexts.Count);
            ContextSpeechBubbleManager.instance.SetContextBubble(3, speechBubbleTransform, localizedTexts[index].GetLocalizedString(), true);
        }
        

    }
}
