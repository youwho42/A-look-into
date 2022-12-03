using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributesDisplay : MonoBehaviour
{
    public Slider bounceSlider;
    public Slider agencySlider;
    //PlayerInformation playerInformation;

    private void Start()
    {
        //playerInformation = PlayerInformation.instance;
        GameEventManager.onStatUpdateEvent.AddListener(UpdateStatsUI);
        UpdateStatsUI();
    }
    private void UpdateStatsUI()
    {
        bounceSlider.value = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce");
        agencySlider.value = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Agency");
    }
    private void OnDestroy()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateStatsUI);
    }
}
