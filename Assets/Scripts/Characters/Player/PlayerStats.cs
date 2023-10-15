using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public QA_AttributeHandler playerAttributes;
    public float maxBounce;

    private void Start()
    {
        
        playerAttributes.AddAttribute("Bounce");
        playerAttributes.AddAttribute("Agency");
        playerAttributes.AddAttribute("Luck");
        playerAttributes.AddAttribute("Gumption");
        playerAttributes.SetAttributeValue("Bounce", maxBounce);
        playerAttributes.SetAttributeValue("Agency", 0);
        playerAttributes.SetAttributeValue("Luck", 5);
        playerAttributes.SetAttributeValue("Gumption", 0);
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    public void AddToStat(string attributeName, float amountToAdd)
    {
        float e = playerAttributes.GetAttributeValue(attributeName);
        e += amountToAdd;
        if(attributeName == "Bounce")
            e = Mathf.Clamp(e, 0, maxBounce);
        playerAttributes.SetAttributeValue(attributeName, e);
        GameEventManager.onStatUpdateEvent.Invoke();

        //NotificationManager.instance.SetNewNotification($"You gained {amountToAdd} {attributeName}", NotificationManager.NotificationType.Agency);
    }
   
    public void AddToAgency(float energyToAdd)
    {
        
        float e = playerAttributes.GetAttributeValue("Agency");
        e += energyToAdd;
        playerAttributes.SetAttributeValue("Agency", e);
        GameEventManager.onStatUpdateEvent.Invoke();
        Notifications.instance.SetNewNotification("", null, (int)energyToAdd, NotificationsType.Agency);

        //NotificationManager.instance.SetNewNotification($"You gained {energyToAdd} Agency", NotificationManager.NotificationType.Agency);
    }
    public void AddToBounce(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Bounce");
        e += energyToAdd;
        e = Mathf.Clamp(e, 0, maxBounce);
        playerAttributes.SetAttributeValue("Bounce", e);
        GameEventManager.onStatUpdateEvent.Invoke();
    }
    public void AddPlayerLuck(float luckToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Luck");
        e += luckToAdd;
        playerAttributes.SetAttributeValue("Luck", e);
        GameEventManager.onStatUpdateEvent.Invoke();
    }
    public void RemoveFromAgency(float energyToAdd)
    {
    
        float e = playerAttributes.GetAttributeValue("Agency");
        e -= energyToAdd;
        playerAttributes.SetAttributeValue("Agency", e);
        GameEventManager.onStatUpdateEvent.Invoke();
    }
    public void RemoveFromBounce(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Bounce");
        e -= energyToAdd;
        e = Mathf.Clamp(e, 0, maxBounce);
        playerAttributes.SetAttributeValue("Bounce", e);
        GameEventManager.onStatUpdateEvent.Invoke();
    }
    public void RemovePlayerLuck(float luckToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Luck");
        e -= luckToAdd;
        playerAttributes.SetAttributeValue("Luck", e);
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    public float GetPlayerLuck(float luckToAdd)
    {
        float l = (luckToAdd / 100) * playerAttributes.GetAttributeValue("Luck");
        return luckToAdd + l;
        //GameEventManager.onStatUpdateEvent.Invoke();
    }
}
