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


    public void Fix(List<FixableAreaIngredient> ingredients)
    {
        if (!isFixing)
            StartCoroutine(FixCo(ingredients));
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
        
        isFixing = true;
        RemoveItemsFromInventory(ingredients);
        fixingEffect.Play();

        // seven seconds to spare...
        // fade out the mud for 3 secs
        //fade in the farm area for 3 secs
        // chill for a sec


        plantingArea.farmAreaActive = true;
        plantingArea.SetFarmAreaActive(true);
        

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
