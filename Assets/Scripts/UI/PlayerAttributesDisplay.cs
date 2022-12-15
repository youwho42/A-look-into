using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributesDisplay : MonoBehaviour
{
    public Slider bounceSlider;
    public TextMeshProUGUI agencyText;
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
        agencyText.text = PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Agency").ToString();
    }
    private void OnDestroy()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateStatsUI);
    }
}
