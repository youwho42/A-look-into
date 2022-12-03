using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTempleStation : Interactable
{
    public GameObject purpleRain;
    public List<GameObject> firesToLight = new List<GameObject>();
    [HideInInspector]
    public bool isActivated;
    public override void Start()
    {
        base.Start();
        
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isActivated)
        {
            if (InteractCostReward())
            {
                Destroy(purpleRain);
                LightFires(true);
            }
        }
        
    }

    public void LightFires(bool lit)
    {
        isActivated = lit;
        foreach (var fire in firesToLight)
        {
            fire.SetActive(lit);
        }
    }

    bool InteractCostReward()
    {
        float agency = playerInformation.playerStats.playerAttributes.GetAttributeValue("Agency");
        if (agency >= gameEnergyCost)
            return true;

        NotificationManager.instance.SetNewNotification("You need " + gameEnergyCost + " Agency to do this.");
        return false;
    }

}
