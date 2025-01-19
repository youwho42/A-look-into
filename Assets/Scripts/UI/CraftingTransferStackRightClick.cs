using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingTransferStackRightClick : MonoBehaviour, IPointerClickHandler
{
    public CraftingStationFuelInventorySlot slot;

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            slot.TransferStack();
        }
    }
}
