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



    bool canStartGame;
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
        if (playerInformation.playerStats.playerAttributes.GetAttributeValue("PlayerEnergy") >= playerEnergyCost)
        {
            playerInformation.playerStats.AddGameEnergy(gameEnergyReward);
            playerInformation.playerStats.RemovePlayerEnergy(playerEnergyCost);
            return true;
        }

        NotificationManager.instance.SetNewNotification("You are missing Yellow Bar stuff to fix this.");
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
