using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Klaxon.GravitySystem;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
    public class InteractWithInteractable : MonoBehaviour
    {
        List<Interactable> currentInteractables = new List<Interactable>();
        public float interactableRadius;
        public Vector3 pickupOffset;
        public LayerMask interactableLayer;


        Vector3 canvasOffset;
        public GravityItemMovementControllerNew playermovement;


        public GameObject interactCanvas;
        public GameObject interactUI;
        public TextMeshProUGUI interactVerb;
        public Slider interactSlider;
        [SerializeField] private InputActionReference holdButton;

        UIScreenManager uiManager;
        PlayerInformation player;

        private float currentHoldTime = 0f;
        public bool isHolding = false;

        private void OnEnable()
        {
            holdButton.action.started += OnHoldButtonPerformed;
            holdButton.action.canceled += OnHoldButtonCanceled;
        }

        private void OnDisable()
        {
            holdButton.action.started -= OnHoldButtonPerformed;
            holdButton.action.canceled -= OnHoldButtonCanceled;
        }


        

        private void Start()
        {
            player = PlayerInformation.instance;
            uiManager = UIScreenManager.instance;
            interactSlider.maxValue = InputSystem.settings.defaultHoldTime;
        }



        private void OnHoldButtonPerformed(InputAction.CallbackContext context)
        {

            isHolding = true;
        }

        private void OnHoldButtonCanceled(InputAction.CallbackContext context)
        {
            isHolding = false;
            currentHoldTime = 0f;
            interactSlider.value = currentHoldTime;
        }



        private void Update()
        {
            interactSlider.gameObject.SetActive(false);
            interactUI.SetActive(false);

            if (uiManager.GetCurrentUI() != UIScreenType.None)
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
                        if (colliders[i].transform.position.z == transform.position.z)
                            currentInteractables.Add(colliders[i].gameObject.GetComponent<Interactable>());

                    }
                }

            }

            if (isHolding)
            {
                currentHoldTime += Time.deltaTime;
                interactSlider.value = currentHoldTime;
            }


            DisplayUI();
        }

        void DisplayUI()
        {
            //interactSlider.gameObject.SetActive(false);
            //interactUI.SetActive(false);
            if (player.isSitting || !player.playerController.isGrounded)
                return;

            if (currentInteractables.Count > 0)
            {
                Interactable closest = GetNearestInteractable(currentInteractables);

                if (closest == null)
                    return;

                if (closest.TryGetComponent(out SpriteRenderer renderer))
                {
                    canvasOffset = new Vector3(0, renderer.bounds.size.y / 2, 1);
                }
                else
                {
                    var rend = closest.GetComponentInChildren<SpriteRenderer>();
                    if (rend != null)
                        canvasOffset = new Vector3(0, rend.transform.localPosition.y + rend.bounds.extents.y , 1);
                    else
                        canvasOffset = Vector3.forward;
                    
                }
                closest.SetInteractVerb();
                string buttTap = player.playerInput.currentControlScheme == "Gamepad" ? "-X-" : "-E-";
                string action="";
                if (closest.localizedInteractVerb != null)
                    action = $"{buttTap} {closest.localizedInteractVerb.GetLocalizedString()}";
                
                interactCanvas.transform.position = closest.transform.position + canvasOffset;
                string buttHold = player.playerInput.currentControlScheme == "Gamepad" ? "-Y-" : "-F-";
                string hold = LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Hold");
                if (closest.hasLongInteract)
                {
                    string itemName = "";
                    if (closest.localizedItemName != null)
                        itemName = closest.localizedItemName.GetLocalizedString();
                    action += $"\n {hold} {buttHold} {closest.longInteractVerb.GetLocalizedString()} {itemName}";
                    interactSlider.gameObject.SetActive(true);
                }
                interactVerb.text = action;

                interactUI.SetActive(true);


            }
        }

        public void Interact()
        {
            if (player.isSitting || !player.playerController.isGrounded || isHolding)
                return;
            if (currentInteractables.Count > 0)
            {
                var interactable = GetNearestInteractable(currentInteractables);
                if (interactable == null)
                    return;
                if (interactable.canInteract)
                    interactable.Interact(gameObject);
            }
        }

        public void LongInteract()
        {
            if (player.isSitting || !player.playerController.isGrounded)
                return;
            if (currentInteractables.Count > 0)
            {
                var interactable = GetNearestInteractable(currentInteractables);
                if (interactable.canInteract && interactable.hasLongInteract)
                    interactable.LongInteract(gameObject);

            }
        }

        public Interactable GetNearestInteractable(List<Interactable> items)
        {
            // Find nearest item.
            Interactable nearest = null;
            float distance = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].canInteract)
                    continue;
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

}