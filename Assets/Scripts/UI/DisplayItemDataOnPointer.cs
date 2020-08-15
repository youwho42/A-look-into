using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class DisplayItemDataOnPointer : MonoBehaviour, IPointerEnterHandler
{
    public TextMeshProUGUI displayTextName;
    public GameObject displayTextObject;
    public InventoryDisplaySlot displaySlot;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // Do something.
        if(displaySlot.item != null)
        {
            displayTextName.text = displaySlot.item.Name;
            displayTextObject.SetActive(true);
        }
        
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // Do something.
        
        displayTextObject.SetActive(false);
        
        
    }
}
