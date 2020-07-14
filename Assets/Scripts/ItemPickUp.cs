using System;
using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickUp : MonoBehaviour
{
    AudioManager audioManager;
    List<QI_Item> currentItems = new List<QI_Item>();
    QI_Inventory inventoryToAddTo;
    
    
    public float pickUpRadius;
    public LayerMask pickupLayer;

    private void Start()
    {
        audioManager = AudioManager.instance;
        inventoryToAddTo = GetComponent<QI_Inventory>();
    }
    
    private void Update()
    {
        currentItems.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpRadius, pickupLayer);

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (!currentItems.Contains(collider.GetComponent<QI_Item>()))
                {
                    currentItems.Add(collider.GetComponent<QI_Item>());
                }
            }
        }

        GetInput();

        
    }

    public void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItems.Count > 0)
        {
            DestroyNearest(currentItems);
        }
    }

    public void DestroyNearest(List<QI_Item> items)
    {
        // Find nearest item.
        QI_Item nearest = null;
        float distance = 0;

        for (int i = 0; i < items.Count; i++)
        {
            float tempDistance = Vector3.Distance(transform.position, items[i].transform.position);
            if (nearest == null || tempDistance < distance)
            {
                nearest = items[i];
                distance = tempDistance;
            }
        }
        if(audioManager.CompareSoundNames("PickUp-" + nearest.Data.Name))
        {
            audioManager.PlaySound("PickUp-" + nearest.Data.Name);
        }
        else
        {
            audioManager.PlaySound("PickUp-Default");
        }
        
        
        inventoryToAddTo.AddItem(nearest.Data, 1);
        // Remove from list.
        items.Remove(nearest);

        // Destroy object.
        Destroy(nearest.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
