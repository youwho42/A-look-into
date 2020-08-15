using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{

    private int dayLength;   //in minutes
    private int dayStart;
    private int nightStart;   //also in minutes
    [Range(0, 1440)]
    public int currentTime;
    public float cycleSpeed;
    private bool isDay;
    
    public Light2D sun;
    

    // Day and Night Script for 2d,
    // Unity needs one empty GameObject (earth) and one Light (sun)
    // make the sun a child of the earth
    // reset the earth position to 0,0,0 and move the sun to -200,0,0
    // attach script to sun
    // add sun and earth to script publics
    // set sun to directional light and y angle to 90


    void Start()
    {
        isDay = true;
        dayLength = 1440;
        dayStart = 300;
        nightStart = 1200;
        
        StartCoroutine(TimeOfDay());
        
    }

    void Update()
    {

        if (currentTime > 0 && currentTime < dayStart)
        {
            if (isDay)
            {
                isDay = false;
                sun.intensity = .3f;
            }
            
        }
        else if (currentTime >= dayStart && currentTime < nightStart)
        {
            if (!isDay)
            {
                isDay = true;
                StartCoroutine(ChangeLight(1.2f));
            }
            
           
        }
        else if (currentTime >= nightStart && currentTime < dayLength)
        {
            if (isDay)
            {
                isDay = false;
                StartCoroutine(ChangeLight(0.3f));
            }
            
        }
        else if (currentTime >= dayLength)
        {
            currentTime = 0;
        }
        
        
    }

    IEnumerator ChangeLight(float amount)
    {
        float elapsedTime = 0;
        float waitTime = 40f;
        float intensity = sun.intensity;
        while (elapsedTime < waitTime)
        {
            sun.intensity = Mathf.Lerp(intensity, amount, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        sun.intensity = amount;
        yield return null;
    }

    IEnumerator TimeOfDay()
    {
        while (true)
        {
            currentTime += 1;
            int hours = Mathf.RoundToInt(currentTime / 60);
            int minutes = currentTime % 60;
            
            yield return new WaitForSeconds(1F / cycleSpeed);
        }
    }
}