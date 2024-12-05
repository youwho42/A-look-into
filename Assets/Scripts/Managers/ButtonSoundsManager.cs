using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundsManager : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None); // parameter makes it include inactive UI elements with buttons
        foreach (Button b in buttons)
        {
            EventTrigger trigger = null;
            if (b.gameObject.TryGetComponent(out EventTrigger t))
                trigger = t;
            else
                trigger = b.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry clickEvent = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerDown
            };
            clickEvent.callback.AddListener(ButtonClickSound);
            trigger.triggers.Add(clickEvent);
            EventTrigger.Entry releaseEvent = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerUp
            };
            releaseEvent.callback.AddListener(ButtonClickSound);
            trigger.triggers.Add(releaseEvent);
        }
    }
    
    public void ButtonClickSound(BaseEventData eventData)
    {
        int num = Random.Range(1, 4);
        AudioManager.instance.PlaySound($"ButtonClick{num}");
    }
}
