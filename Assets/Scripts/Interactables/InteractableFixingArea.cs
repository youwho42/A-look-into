using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FixableAreaIngredient
{
    public QI_ItemData item;
    public int amount;
}

public class InteractableFixingArea : Interactable
{



    
    public List<FixableAreaIngredient> ingredients = new List<FixableAreaIngredient>();
    

    public override void Start()
    {
        base.Start();

    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        bool ingr = CheckForIngredients();
        bool cost = InteractCostReward();
        if (ingr && cost)
        {
            GetComponent<IFixArea>().Fix(ingredients);
        }
    }

    
    public bool CheckForIngredients()
    {
        bool hasAll = true;
        foreach (var ingredient in ingredients)
        {
            
            int t = playerInformation.GetTotalInventoryQuantity(ingredient.item);
            if (t < ingredient.amount)
            {
                string plural = ingredient.amount - t == 1 ? "" : "'s";
                NotificationManager.instance.SetNewNotification($"Missing {ingredient.amount - t} {ingredient.item.Name}{plural}", NotificationManager.NotificationType.Warning);
                hasAll = false;
            }
            
        }
        return hasAll;
    }

    bool InteractCostReward()
    {
        float agency = playerInformation.playerStats.playerAttributes.GetAttributeValue("Agency");
        if (agency >= agencyCost)
            return true;
        

        NotificationManager.instance.SetNewNotification($"{agencyCost} Agency missing", NotificationManager.NotificationType.Warning);
        return false;
    }


    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }
}
