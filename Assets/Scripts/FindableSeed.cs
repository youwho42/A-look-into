using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
public class FindableSeed : MonoBehaviour
{
    public QI_ItemDatabase seedDatabase;
    
    public QI_ItemData seed;

    public void Start()
    {
        seed = seedDatabase.GetRandomWeightedItem();
    }
    
}
