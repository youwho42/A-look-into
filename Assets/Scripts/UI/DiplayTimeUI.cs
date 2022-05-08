using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiplayTimeUI : MonoBehaviour
{

    RealTimeDayNightCycle dayNightCycle;
    public TextMeshProUGUI text;

    private void Start()
    {

        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetTime);
    }

    void SetTime(int tick)
    {
        text.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetTime);
    }
}
