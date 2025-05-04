using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelBoxManager : MonoBehaviour
{
    public static SquirrelBoxManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public QI_Inventory inventory;
}
