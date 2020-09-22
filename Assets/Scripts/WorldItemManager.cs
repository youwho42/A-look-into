using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    public static WorldItemManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public Dictionary<string, int> worldObjectsDictionary = new Dictionary<string, int>();

    public void AddItemToWorldItemDictionary(string itemName, int quantity)
    {
        if (worldObjectsDictionary.ContainsKey(itemName))
            worldObjectsDictionary[itemName] += quantity;
        else
            worldObjectsDictionary.Add(itemName, quantity);
    }
    public void RemoveItemFromWorldItemDictionary(string itemName, int quantity)
    {
        if (worldObjectsDictionary.ContainsKey(itemName))
            worldObjectsDictionary[itemName] -= quantity;
        if(worldObjectsDictionary.TryGetValue(itemName, out int amount))
        {
            if (amount == 0)
                worldObjectsDictionary.Remove(itemName);
        }
    }
    public int GetWorldObjectAmount()
    {
        int amount = 0;
        foreach (KeyValuePair<string, int> dictItem in worldObjectsDictionary)
        {
            amount += dictItem.Value;
        }
        return amount;
    }
}
