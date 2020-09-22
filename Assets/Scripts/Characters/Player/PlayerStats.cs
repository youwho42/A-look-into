using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public QA_AttributeHandler playerAttributes;
    DayNightCycle dayNightCycle;
    public int minutes;
    bool dehydrating = false;


    private void Start()
    {
        dayNightCycle = DayNightCycle.instance;
        playerAttributes.AddAttribute("Energy");
        playerAttributes.AddAttribute("Hydration");
        playerAttributes.SetAttributeValue("Energy", 10);
        playerAttributes.SetAttributeValue("Hydration", 100);
    }
    private void Update()
    {
        minutes = dayNightCycle.currentTimeRaw % 30;
        if (!dehydrating)
        {
            if (minutes == 9)
                Dehydrate();
            
        }
        if (minutes != 9)
            dehydrating = false;
    }
    void Dehydrate()
    {
        dehydrating = true;
        float hydration = playerAttributes.GetAttributeValue("Hydration");
        hydration -= 1;
        playerAttributes.SetAttributeValue("Hydration", hydration);
    }
}
