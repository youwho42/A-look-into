using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using QuantumTek.EncryptedSave;

public class SavePlayerCraftingRecipes : SaveableManager
{

    public QI_CraftingRecipeDatabase allRecipesDatabase;
    public QI_CraftingRecipeDatabase playerRecipesDatabase;
    List<string> recipes = new List<string>();
    public override void Save()
    {
        
        base.Save();
        recipes.Clear();
        foreach (var item in playerRecipesDatabase.CraftingRecipes)
        {
            recipes.Add(item.Name);
        }
        ES_Save.Save(recipes, saveableName);
    }

    public override void Load()
    {
        base.Load();
        recipes.Clear();
        playerRecipesDatabase.CraftingRecipes.Clear();
        recipes = ES_Save.Load<List<string>>(saveableName);
        foreach (var item in recipes)
        {
            playerRecipesDatabase.CraftingRecipes.Add(allRecipesDatabase.GetCraftingRecipe(item));
        }
    }
}
