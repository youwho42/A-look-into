using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Klaxon.GravitySystem;
using UnityEngine.InputSystem;

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
        if (PlayerInformation.instance.playerInput.isInUI)
            return;

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
    }

    void DisplayUI()
    {
        interactUI.SetActive(false);
        
        if (currentInteractables.Count > 0)
        {
            Interactable closest = GetNearestInteractable(currentInteractables);
            if (!closest.canInteract)
                return;
            if (closest.TryGetComponent(out SpriteRenderer renderer))
            {
                canvasOffset = new Vector3(0, renderer.bounds.size.y / 2, 1);
            } else
            {
                canvasOffset = new Vector3(0, closest.GetComponentInChildren<SpriteRenderer>().bounds.size.y / 2, 1);
            }

            string action = PlayerInformation.instance.playerInput.currentControlScheme == "Gamepad" ? "-X-" : "-E-";    
            interactCanvas.transform.position = closest.transform.position + canvasOffset;
            interactVerb.text = $"{action} {closest.interactVerb}";
            interactUI.SetActive(true);
            
            
        }
    }

    public void Interact()
    {
        if (currentInteractables.Count > 0)
        {
            var interactable = GetNearestInteractable(currentInteractables);
            if (interactable.canInteract)
                interactable.Interact(gameObject);
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
