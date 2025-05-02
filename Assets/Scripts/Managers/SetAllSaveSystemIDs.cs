using UnityEngine;
using Klaxon.SaveSystem;
using System.Collections.Generic;

public class SetAllSaveSystemIDs : MonoBehaviour
{

    [ContextMenu("Set All Unset IDs")]
    void SetAllIDs()
    {
        int amount = 0;
        var allSaveables = FindObjectsByType<SaveableWorldEntity>(FindObjectsSortMode.None);
        for (int i = 0; i < allSaveables.Length; i++)
        {
            if (allSaveables[i].ID == "")
            {
                allSaveables[i].GenerateId();
                amount++;
            }
        }
        Debug.LogWarning($"{amount} saveable items without ID");
    }

    [ContextMenu("Check IDs for Duplicates")]
    void CheckForDuplicates()
    {
        var allSaveables = FindObjectsByType<SaveableWorldEntity>(FindObjectsSortMode.None);
        int amount = 0;
        for (int i = 0; i < allSaveables.Length; i++)
        {
            for (int j = 0; j < allSaveables.Length; j++)
            {
                if (i == j)
                    continue;
                if (allSaveables[i].ID == allSaveables[j].ID)
                {
                    amount++;
                    allSaveables[j].GenerateId();
                }
            }
        }
        Debug.LogWarning($"{amount} IDs were duplicates");
    }
    
}
