using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemKeepSelected : MonoBehaviour
{
    private EventSystem eventSystem;
    private GameObject lastSelected = null;
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (eventSystem != null)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                lastSelected = eventSystem.currentSelectedGameObject;
            }
            else
            {
                eventSystem.SetSelectedGameObject(lastSelected);
            }
        }
    }
}