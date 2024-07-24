using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickButtonEvent : MonoBehaviour, IPointerClickHandler
{

    public ContainerDisplaySlot containerDisplaySlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            containerDisplaySlot.TransferItem(false);
        }
    }

}
