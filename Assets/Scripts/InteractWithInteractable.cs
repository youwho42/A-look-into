using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InteractWithInteractable : MonoBehaviour
{
    List<Interactable> currentInteractables = new List<Interactable>();
    public float interactableRadius;
    public LayerMask interactableLayer;

    public GameObject interactCanvas;
    public GameObject interactUI;
    public TextMeshProUGUI interactVerb;

    Vector3 canvasOffset;

    

    private void Update()
    {
        currentInteractables.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactableRadius, interactableLayer);

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Interactable>() != null)
                {
                    currentInteractables.Add(colliders[i].gameObject.GetComponent<Interactable>());
                    
                }
            }
            
        }

        DisplayUI();
        GetInput();

    }

    void DisplayUI()
    {
        interactUI.SetActive(false);
        
        if (currentInteractables.Count > 0)
        {
            Interactable closest = GetNearestInteractable(currentInteractables);
            if (closest.canInteract)
            {
                canvasOffset = new Vector3(0, closest.GetComponent<SpriteRenderer>().bounds.size.y / 2, 1);
                interactCanvas.transform.position = closest.transform.position + canvasOffset;
                interactVerb.text = closest.interactVerb;
                interactUI.SetActive(true);
            }
            
        }
    }

    public void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractables.Count > 0)
        {
            if (GetNearestInteractable(currentInteractables).canInteract)
            {
                GetNearestInteractable(currentInteractables).Interact(gameObject);
                
            }
        }
    }

    public Interactable GetNearestInteractable(List<Interactable> items)
    {
        // Find nearest item.
        Interactable nearest = null;
        float distance = 0;
        
        for (int i = 0; i < items.Count; i++)
        {
            
            float tempDistance = Vector3.Distance(transform.position, items[i].gameObject.transform.position);
            if (nearest == null || tempDistance < distance)
            {
                nearest = items[i];
                distance = tempDistance;
            }
        }
        
        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactableRadius);
    }
}
