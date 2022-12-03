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
        if (CheckForIngredients() && InteractCostReward())
        {
            //playerInformation.playerStats.RemoveGameEnergy(gameEnergyCost);
            GetComponent<IFixArea>().Fix(ingredients);
        }
    }

    
    public bool CheckForIngredients()
    {
        foreach (var ingredient in ingredients)
        {
            int t = playerInformation.GetTotalInventoryQuantity(ingredient.item);
            if (t < ingredient.amount)
            {
                NotificationManager.instance.SetNewNotification("You are missing " + (ingredient.amount - t) + " " + ingredient.item.Name + " to fix this.");
                return false;
            }
            
        }
        return true;
    }

    bool InteractCostReward()
    {
        float agency = playerInformation.playerStats.playerAttributes.GetAttributeValue("Agency");
        if (agency >= gameEnergyCost)
            return true;
        

        NotificationManager.instance.SetNewNotification("You need " + gameEnergyCost + " Agency to fix this.");
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
