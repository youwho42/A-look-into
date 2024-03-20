using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISelectHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSelected = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
    }
}