using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class RecipesToCSV : MonoBehaviour
{
    public QI_CraftingRecipeDatabase allRecipesDatabase;
    public QI_ItemDatabase allItemsDatabase;
    public List<EquipmentAxeData> axes = new List<EquipmentAxeData>();
    string fileName = "";
    public List<InteractableCraftingStation> allCraftingStations = new List<InteractableCraftingStation>();

    [ContextMenu("Create File")]
    public void CreateFile()
    {
        fileName = Application.dataPath + "/RecipesGraph.csv";
        WriteFile();
    }


    void WriteFile()
    {
        if (allRecipesDatabase.CraftingRecipes.Count > 0)
        {
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine("Item, Ingredients, Researched Item, Crafting Station, Minigames per, Minigame Subper");
            tw.Close();

            tw = new StreamWriter(fileName, true);

            foreach (var recipe in allRecipesDatabase.CraftingRecipes)
            {
                string ingredients = GetIngredients(recipe);
                string researchedItem = GetResearchedItemSource(recipe);
                string craftingStation = GetCraftingStation(recipe);
                string minigame = GetMinigameQuantities(recipe);
                string subAmount = "";

                foreach (var ingredient in recipe.Ingredients)
                {
                    foreach (var r in allRecipesDatabase.CraftingRecipes)
                    {
                        if(r.Product.Item == ingredient.Item)
                        {
                            
                            string m = GetMinigameQuantities(r);
                            var numeric = new String(m.Where(char.IsDigit).ToArray());
                            var type = new String(m.Where(char.IsLetter).ToArray());
                            if(numeric != "")
                                subAmount += $"{type}=>{ingredient.Amount * Int32.Parse(numeric)} / ";
                            
                            
                        }
                    }
                }



                tw.WriteLine($"- {recipe.Product.Item.Name}, {ingredients}, {researchedItem}, {craftingStation}, {minigame}, {subAmount}");
            }
            tw.Close();
        }

    }

    private string GetMinigameQuantities(QI_CraftingRecipe recipe)
    {
        string minigame = "";
        foreach (var ingredient in recipe.Ingredients)
        {
            foreach (var axe in axes)
            {
                if (axe.gatherItemData.Contains(ingredient.Item))
                {
                    string type = "";
                    int gameAmount = 0;
                    if (axe.miniGameType == MiniGameType.Ore)
                    {
                        type = axe.miniGameType.ToString();
                        gameAmount = Mathf.CeilToInt(ingredient.Amount / 5.0f);

                    }
                    else if (axe.miniGameType == MiniGameType.Wood)
                    {
                        type = axe.miniGameType.ToString();
                        gameAmount = Mathf.CeilToInt(ingredient.Amount / 9.0f);
                    }
                    minigame += $"{type}=>{gameAmount} / ";
                    break;
                }
            }
        }

        return minigame;
    }
    string GetResearchedItemSource(QI_CraftingRecipe recipe)
    {
        string source = "";
        foreach (var item in allItemsDatabase.Items)
        {
            foreach (var r in item.ResearchRecipes)
            {
                if (r.recipe == recipe)
                    source += $"-{item.Name} ";
            }
        }

        return source == "" ? "None" : source;
    }

    string GetCraftingStation(QI_CraftingRecipe recipe)
    {

        foreach (var station in allCraftingStations)
        {
            foreach (var r in station.recipeDatabase.CraftingRecipes)
            {
                if (r == recipe)
                {
                    if(station.TryGetComponent(out QI_Item item))
                        return $"{item.Data.localizedName.GetLocalizedString()}";
                    else
                        return "Probably Crafting Station Basic";
                }
                    
            }
        }
        return "None assigned";
    }

    private static string GetIngredients(QI_CraftingRecipe recipe)
    {
        string ingredients = "";
        foreach (var item in recipe.Ingredients)
        {
            string ingredient = $"- {item.Item.Name} x{item.Amount}";
            ingredients += ingredient;
        }

        return ingredients;
    }
}
