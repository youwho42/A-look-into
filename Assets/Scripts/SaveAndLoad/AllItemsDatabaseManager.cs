using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using Klaxon.UndertakingSystem;
using Klaxon.ConversationSystem;

public class AllItemsDatabaseManager : MonoBehaviour
{
    public static AllItemsDatabaseManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
            
    }

    public QI_ItemDatabase allItemsDatabase;
    public QI_CraftingRecipeDatabase allRecipesDatabase;
    public UndertakingDatabase allUndertakingsDatabase;
    public ConversationDatabase allConversationsDatabase;

    public void ResetItemsDatabase()
    {
        allItemsDatabase.SetAllItems();
        allRecipesDatabase.SetAllRecipes();
        allUndertakingsDatabase.SetAllUndertakings();
        allConversationsDatabase.SetAllCoversations();
    }
}
