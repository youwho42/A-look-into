 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RightClickButtonEvent : MonoBehaviour, IPointerClickHandler
{

    public ContainerDisplaySlot containerDisplaySlot;
    [SerializeField] 
    private InputActionReference holdShiftButton;
    bool isShifty;

    

    void Start()
    {
        holdShiftButton.action.started += SetShiftHeld;
        holdShiftButton.action.canceled += SetShiftHeld;
    }

    void OnDisable()
    {
        holdShiftButton.action.started -= SetShiftHeld;
        holdShiftButton.action.canceled -= SetShiftHeld;
    }

    public void SetShiftHeld(InputAction.CallbackContext context)
    {
        isShifty = !isShifty;
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(isShifty)
                containerDisplaySlot.TransferItem(false);
            else
                containerDisplaySlot.TransferStack();
        }
    }

}
