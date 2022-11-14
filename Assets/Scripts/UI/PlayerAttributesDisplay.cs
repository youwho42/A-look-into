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
        GameEventManager.onStatUpdateEvent.AddListener(UpdateStatsUI);
        UpdateStatsUI();
    }
    private void UpdateStatsUI()
    {
        energySlider.value = playerInformation.playerStats.playerAttributes.GetAttributeValue("Bounce");
        hydrationSlider.value = playerInformation.playerStats.playerAttributes.GetAttributeValue("Agency");
    }
    private void OnDestroy()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateStatsUI);
    }
}
