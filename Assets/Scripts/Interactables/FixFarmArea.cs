using Klaxon.SAP;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixFarmArea : MonoBehaviour, IFixArea
{
    public PlantingArea plantingArea;
    bool isFixing;
    public ParticleSystem fixingEffect;
    public CompleteTaskObject undertakingObject;
    public SAP_Condition worldStateEffect;
    SAP_WorldBeliefStates worldState;

    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if (undertakingObject.undertaking != null)
        {
            if (undertakingObject.undertaking.CurrentState != UndertakingState.Active)
            {
                NotificationManager.instance.SetNewNotification($"This doesn't work", NotificationManager.NotificationType.Warning);
                return false;
            }
        }
        if (!isFixing)
            StartCoroutine(FixCo(ingredients));
        return true;
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
        worldState = SAP_WorldBeliefStates.instance;
        isFixing = true;
        RemoveItemsFromInventory(ingredients);
        fixingEffect.Play();

        // seven seconds to spare...
        // fade out the mud for 3 secs
        //fade in the farm area for 3 secs
        // chill for a sec
        yield return new WaitForSeconds(6);

        plantingArea.farmAreaActive = true;
        plantingArea.SetFarmAreaActive(true);
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        if(worldStateEffect.Condition != "")
            worldState.SetWorldState(worldStateEffect.Condition, worldStateEffect.State);

        yield return null;
    }
    void RemoveItemsFromInventory(List<FixableAreaIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item.Name, ingredient.amount);
            NotificationManager.instance.SetNewNotification($"{ingredient.amount} {ingredient.item.Name} removed", NotificationManager.NotificationType.Inventory);

        }
    }
}
