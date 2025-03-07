using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Klaxon.Interactable;


public class FixFarmArea : MonoBehaviour, IFixArea
{
    public PlantingArea plantingArea;
    bool isFixing;
    public ParticleSystem fixingEffect;
    public CompleteTaskObject undertakingObject;
    public GOAD_ScriptableCondition worldStateEffect;
    public GOAD_ScriptableCondition worldStatePrecondition;
    GOAD_WorldBeliefStates worldState;

    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        if (worldStatePrecondition != null)
        {
            if (!GOAD_WorldBeliefStates.instance.HasState(worldStatePrecondition.Condition, worldStatePrecondition.State))
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Unavailable"), null, 0, NotificationsType.Warning);
                return false;
            }
        }

        if (undertakingObject.undertaking != null)
        {
            if (undertakingObject.undertaking.CurrentState != UndertakingState.Active)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Unavailable"), null, 0, NotificationsType.Warning);
                return false;
            }
        }
        if (!isFixing)
            StartCoroutine(FixCo(ingredients));
        return true;
    }

    IEnumerator FixCo(List<FixableAreaIngredient> ingredients)
    {
        var player = PlayerInformation.instance;
        player.playerInput.isInUI = true;
        player.animatePlayerScript.SetCraftAnimation(true);

        worldState = GOAD_WorldBeliefStates.instance;
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
        if(worldStateEffect != null)
            worldState.SetWorldState(worldStateEffect.Condition, worldStateEffect.State);
        player.playerInput.isInUI = false;
        player.animatePlayerScript.SetCraftAnimation(false);
        yield return null;
    }
    void RemoveItemsFromInventory(List<FixableAreaIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(ingredient.item.Name, ingredient.amount);
            Notifications.instance.SetNewNotification("", ingredient.item, -ingredient.amount, NotificationsType.Inventory);

            //NotificationManager.instance.SetNewNotification($"{ingredient.amount} {ingredient.item.Name} removed", NotificationManager.NotificationType.Inventory);

        }
    }
}
