using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiplayTimeUI : MonoBehaviour
{

    RealTimeDayNightCycle dayNightCycle;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public Image hourHand;
    public Image minuteHand;

    private void Start()
    {

        dayNightCycle = RealTimeDayNightCycle.instance;
        GameEventManager.onTimeTickEvent.AddListener(SetTime);
    }

    void SetTime(int tick)
    {
        timeText.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
        SetHandRotations();
        dayText.text = $"Day {dayNightCycle.currentDayRaw}";
    }
    

    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetTime);
    }

    void SetHandRotations()
    {
        float minutes = dayNightCycle.minutes;
        float hours = dayNightCycle.currentTimeRaw;
        
        var mR = Mathf.Lerp(0, -360, minutes / 60);
        var hR = Mathf.Lerp(0, -360, (hours % 720) / 720);
        var mRotation = new Vector3(0, 0, mR);
        var hRotation = new Vector3(0, 0, hR);
        minuteHand.transform.eulerAngles = mRotation;
        hourHand.transform.eulerAngles = hRotation;
    }

}
