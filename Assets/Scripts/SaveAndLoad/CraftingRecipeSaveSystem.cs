using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class CraftingRecipeSaveSystem : MonoBehaviour, ISaveable
    {

        public QI_CraftingRecipeDatabase craftingRecipeDatabase;
        public QI_CraftingRecipeDatabase allRecipesDatabase;
        public object CaptureState()
        {
            var recipes = PlayerCrafting.instance;
            List<string> names = new List<string>();

            for (int i = 0; i < recipes.craftingRecipeDatabase.CraftingRecipes.Count; i++)
            {
                names.Add(recipes.craftingRecipeDatabase.CraftingRecipes[i].Name);

            }

            return new SaveData
            {
                itemName = names
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            PlayerCrafting.instance.DestroyAllRecipes();
            for (int i = 0; i < saveData.itemName.Count; i++)
            {
                PlayerCrafting.instance.AddCraftingRecipe(allRecipesDatabase.GetCraftingRecipe(saveData.itemName[i]), true);

            }
        }

        [Serializable]
        private struct SaveData
        {
            public List<string> itemName;


        }

    } 
}