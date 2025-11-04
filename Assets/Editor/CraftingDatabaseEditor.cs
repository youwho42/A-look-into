using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using QuantumTek.QuantumInventory;
using System.Linq;

[CustomEditor(typeof(QI_CraftingRecipeDatabase))]
public class CraftingDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector fields

        QI_CraftingRecipeDatabase scriptableObject = (QI_CraftingRecipeDatabase)target;

        if (GUILayout.Button("Get All Items"))
        {
            scriptableObject.CraftingRecipes.Clear();
            // Perform your custom context menu action here
            scriptableObject.CraftingRecipes = Resources.LoadAll<QI_CraftingRecipe>("Items/").ToList();
            scriptableObject.CraftingRecipes = scriptableObject.CraftingRecipes.OrderBy(x => x.ProductType).ThenBy(x => x.name).ToList();
        }
    }
}
