using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTeleport : MonoBehaviour, IFixArea
{
    public SaveableItemEntity teleportObject;
    
    

    public bool Fix(List<FixableAreaIngredient> ingredients)
    {
        RemoveItemsFromInventory(ingredients);
        NameTeleport();
        return true;
    }
    void NameTeleport()
    {

        SetUpTeleportSystem();
    }

    void SetUpTeleportSystem()
    {
        // instantiate the teleport
        var go = Instantiate(teleportObject, transform.position, Quaternion.identity);

        //get teleport script and set current level
        if (go.TryGetComponent(out Teleport teleporter))
        {
            teleporter.currentLevel = (int)transform.position.z;
            teleporter.teleportName = "";
            teleporter.SetCurrentLevel();
        }
        //generate saveable id
        go.GenerateId();
        Destroy(gameObject);
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
