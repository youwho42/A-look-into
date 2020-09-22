using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SleepDisplay : MonoBehaviour
{
    DayNightCycle dayNightCycle;

    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI sleepUntilTime;
    public Slider slider;


    public void Start()
    {
        
        dayNightCycle = DayNightCycle.instance;

    }

    private void Update()
    {
        currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
        sleepUntilTime.text = string.Format("{0:00}:00", slider.value);
    }

    public void Sleep()
    {
        StartCoroutine("SleepCo");
    }
    IEnumerator SleepCo()
    {
        PlayerInformation.instance.TogglePlayerInput(false);
        int wakeTime = (int)slider.value;
        float originalCycleSpeed = dayNightCycle.cycleSpeed;
        while(dayNightCycle.hours != wakeTime)
        {
            float currentEnergy = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Energy");
            PlayerInformation.instance.playerStats.playerAttributes.SetAttributeValue("Energy", currentEnergy + 0.5f);
            dayNightCycle.cycleSpeed = 100;
            yield return null;
        }
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        gameObject.SetActive(false);
        PlayerInformation.instance.TogglePlayerInput(true);
        yield return null;

    }
}
