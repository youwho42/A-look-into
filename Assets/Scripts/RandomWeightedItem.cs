using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public static class RandomWeightedItem
{


    public static QI_ItemData GetWeightedItem(QI_ItemDatabase itemDatabase)
    {
        float sum = 0;
   
        foreach(var item in itemDatabase.Items)
        {
            sum += item.spawnWeight;
        }
        float randomWeight = 0;
        do
        {
            //No weight on any number?
            if (sum == 0)
                return null;
            randomWeight = Random.Range(0, sum);
        }
        while (randomWeight == sum);
        
        foreach (var item in itemDatabase.Items)
        {
            if (randomWeight < item.spawnWeight)
                return item;
            randomWeight -= item.spawnWeight;
        }
        
        return null;

        
    }

}
