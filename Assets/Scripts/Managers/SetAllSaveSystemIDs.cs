using UnityEngine;
using Klaxon.SaveSystem;

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
}
