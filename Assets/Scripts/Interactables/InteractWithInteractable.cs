using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Klaxon.GravitySystem;

public class InteractWithInteractable : MonoBehaviour
{
    List<Interactable> currentInteractables = new List<Interactable>();
    public float interactableRadius;
    public Vector3 pickupOffset;
    public LayerMask interactableLayer;

    public GameObject interactCanvas;
    public GameObject interactUI;
    public TextMeshProUGUI interactVerb;

    Vector3 canvasOffset;
    public GravityItemMovementControllerNew playermovement;
    

    private void Update()
    {
        currentInteractables.Clear();
        Vector3 pickUpPosition = playermovement.facingRight ? transform.position + pickupOffset : transform.position - pickupOffset;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pickUpPosition, interactableRadius, interactableLayer);

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Interactable>() != null)
                {
                    if(colliders[i].transform.position.z == transform.position.z)
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
            
            if (closest.TryGetComponent(out SpriteRenderer renderer))
            {
                canvasOffset = new Vector3(0, renderer.bounds.size.y / 2, 1);
            } else
            {
                canvasOffset = new Vector3(0, closest.GetComponentInChildren<SpriteRenderer>().bounds.size.y / 2, 1);
            }

                
            interactCanvas.transform.position = closest.transform.position + canvasOffset;
            interactVerb.text = $"-e- {closest.interactVerb}";
            interactUI.SetActive(true);
            
            
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
        Gizmos.DrawWireSphere(transform.position + pickupOffset, interactableRadius);
    }
}
