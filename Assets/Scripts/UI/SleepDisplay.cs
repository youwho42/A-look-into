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
    private Coroutine sleepCoroutine;
    float originalCycleSpeed;
    public void Start()
    {
        
        dayNightCycle = DayNightCycle.instance;
        originalCycleSpeed = dayNightCycle.cycleSpeed;
    }

    private void Update()
    {
        currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
        sleepUntilTime.text = string.Format("{0:00}:00", slider.value);
    }

    public void Sleep()
    {
        sleepCoroutine = StartCoroutine(SleepCo());
        
    }
    public void CancelSleep()
    {
        StopCoroutine(sleepCoroutine);
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        PlayerInformation.instance.TogglePlayerInput(true);
        gameObject.SetActive(false);
    }
    IEnumerator SleepCo()
    {
        PlayerInformation.instance.TogglePlayerInput(false);
        int wakeTime = (int)slider.value;
        
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
