using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SleepDisplayUI : MonoBehaviour
{

    public static SleepDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    DayNightCycle dayNightCycle;

    public GameObject sleepDisplay;
    PlayerInformation playerInformation;
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI sleepUntilTime;
    public Slider slider;
    private Coroutine sleepCoroutine;
    float originalCycleSpeed;
    bool isShowing;
    public void Start()
    {
        playerInformation = PlayerInformation.instance;
        dayNightCycle = DayNightCycle.instance;
        originalCycleSpeed = dayNightCycle.cycleSpeed;
    }

    private void Update()
    {
        if (isShowing)
        {
            currentTime.text = string.Format("{0:00}:{1:00}", dayNightCycle.hours, dayNightCycle.minutes);
            sleepUntilTime.text = string.Format("{0:00}:00", slider.value);
        }
        
    }
    public void ShowUI()
    {
        //UpdateSleepDisplay();
        playerInformation.TogglePlayerInput(false);
        sleepDisplay.SetActive(true);
        isShowing = true;
    }

    public void HideUI()
    {
        playerInformation.TogglePlayerInput(true);
        sleepDisplay.SetActive(false);
        isShowing = false;
    }

    public void Sleep()
    {
        sleepCoroutine = StartCoroutine(SleepCo());
        
    }
    public void CancelSleep()
    {
        if(sleepCoroutine != null)
            StopCoroutine(sleepCoroutine);
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        playerInformation.TogglePlayerInput(true);
        sleepDisplay.SetActive(false);
    }
    IEnumerator SleepCo()
    {
        
        int wakeTime = (int)slider.value;
        
        while(dayNightCycle.hours != wakeTime)
        {
            float currentEnergy = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("PlayerEnergy");
            PlayerInformation.instance.playerStats.playerAttributes.SetAttributeValue("PlayerEnergy", currentEnergy + 0.1f);
            dayNightCycle.cycleSpeed = 200;
            yield return null;
        }
        dayNightCycle.cycleSpeed = originalCycleSpeed;
        sleepDisplay.SetActive(false);
        playerInformation.TogglePlayerInput(true);
        yield return null;

    }
}
