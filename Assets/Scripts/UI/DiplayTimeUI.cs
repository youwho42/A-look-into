using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiplayTimeUI : MonoBehaviour
{

    DayNightCycle dayNightCycle;
    public TextMeshProUGUI text;

    private void Start()
    {
        dayNightCycle = DayNightCycle.instance;
        dayNightCycle.TickEventCallBack.AddListener(SetTime);
    }

    void SetTime(int tick)
    {
        text.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
    }
}
