using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundsManager : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // parameter makes it include inactive UI elements with buttons
        foreach (Button b in buttons)
        {
            //b.onClick.AddListener(ButtonClickSound);
            EventTrigger t = b.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry clickEvent = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerDown
            };
            clickEvent.callback.AddListener(ButtonClickSound);
            t.triggers.Add(clickEvent);
            EventTrigger.Entry releaseEvent = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerUp
            };
            releaseEvent.callback.AddListener(ButtonClickSound);
            t.triggers.Add(releaseEvent);
        }
    }
    
    public void ButtonClickSound(BaseEventData eventData)
    {
        int num = Random.Range(1, 4);
        AudioManager.instance.PlaySound($"ButtonClick{num}");
    }
}
