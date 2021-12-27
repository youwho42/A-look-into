using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTeleport : MonoBehaviour, IFixArea
{
    public SaveableItemEntity teleportObject;



    public void Fix(List<FixableAreaIngredient> ingredients)
    {
        RemoveItemsFromInventory(ingredients);
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

        }
        NotificationManager.instance.SetNewNotification("Items removed from inventory");
    }
}
