using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using QuantumTek.QuantumInventory;

public class ItemsToCSV : MonoBehaviour
{
    public QI_ItemDatabase allItemsDatabase;

    string fileName = "";

    [ContextMenu("Create File")]
    public void CreateFile()
    {
        fileName = Application.dataPath + "/ItemsGraph.csv";
        WriteFile();
    }

    
    void WriteFile()
    {
        if (allItemsDatabase.Items.Count > 0)
        {
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine("Type, Item, Max-Stack, ResearchRecipes");
            tw.Close();

            tw = new StreamWriter(fileName, true);

            foreach (var item in allItemsDatabase.Items)
            {
                string recipes = "";

                
                if (item.ResearchRecipes.Count > 0)
                {
                    foreach (var r in item.ResearchRecipes)
                    {
                        if (r.recipe != null)
                            recipes += $" - {r.recipe.Name}";
                    }
                }
                else
                {
                    recipes = "***";
                }
                

                tw.WriteLine($"{item.Type}, {item.Name}, {item.MaxStack}, {recipes}");
            }
            tw.Close();
        }

    }
}
