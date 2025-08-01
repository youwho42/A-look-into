using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
    public static PlayerCrafting instance;
    public QI_CraftingRecipeDatabase craftingRecipeDatabase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public bool AddCraftingRecipe(QI_CraftingRecipe recipe, bool isFromSave)
    {

        if (!craftingRecipeDatabase.CraftingRecipes.Contains(recipe))
        {
            craftingRecipeDatabase.CraftingRecipes.Add(recipe);
            GameEventManager.onRecipeCompediumUpdateEvent.Invoke();
            if(!isFromSave)
                Notifications.instance.SetNewLargeNotification(null, recipe.Product.Item, recipe, NotificationsType.Compendium);

            return true;
        }
        return false;
    }
    public void DestroyAllRecipes()
    {
        craftingRecipeDatabase.CraftingRecipes.Clear();
    }
    private void OnDisable()
    {
        DestroyAllRecipes();
    }
}
