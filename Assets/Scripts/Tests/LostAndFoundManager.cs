using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostAndFoundManager : MonoBehaviour
{
    public static LostAndFoundManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public QI_Inventory inventory;
}
