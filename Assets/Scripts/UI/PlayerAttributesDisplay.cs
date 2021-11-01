using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributesDisplay : MonoBehaviour
{
    public Slider energySlider;
    public Slider hydrationSlider;
    PlayerInformation playerInformation;

    private void Start()
    {
        playerInformation = PlayerInformation.instance;
    }
    private void Update()
    {
        energySlider.value = playerInformation.playerStats.playerAttributes.GetAttributeValue("PlayerEnergy");
        hydrationSlider.value = playerInformation.playerStats.playerAttributes.GetAttributeValue("GameEnergy");
    }
}
