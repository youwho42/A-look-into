using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundsManager : MonoBehaviour
{

    public static ButtonSoundsManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None); // parameter makes it include inactive UI elements with buttons
        foreach (Button b in buttons)
        {
            AddButton(b);
        }
    }

    private void AddButton(Button b)
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

    public void ButtonClickSound(BaseEventData eventData)
    {
        int num = Random.Range(1, 4);
        AudioManager.instance.PlaySound($"ButtonClick{num}");
    }

    public void AddButtonToSounds(Button b)
    {
        AddButton(b);
    }
}
