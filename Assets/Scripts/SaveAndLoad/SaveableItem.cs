using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Basic saveable item */
public class SaveableItem : MonoBehaviour
{
    [SerializeField]
    private string id = string.Empty;

    public string ID => id;

    [ContextMenu("Generate ID")]
    public void GenerateId() => id = Guid.NewGuid().ToString();

    public QI_ItemData item;
    public void SetID(string newID)
    {
        id = newID;
    }

    public virtual void Save()
    {
        
    }
    public virtual void Load()
    {
       

    }
    
}
