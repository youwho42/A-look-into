using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NPC_UndertakingAvailable : MonoBehaviour
{
    UndertakingHolder undertakingHolder;
    public SpriteRenderer undertakingAvailableIcon;

    private void Start()
    {
        undertakingHolder = GetComponent<UndertakingHolder>();
        SetUndertakingIcon();
    }
    private void OnEnable()
    {
        GameEventManager.onUndertakingsUpdateEvent.AddListener(SetUndertakingIcon);
    }
    private void OnDisable()
    {
        GameEventManager.onUndertakingsUpdateEvent.RemoveListener(SetUndertakingIcon);
    }


    void SetUndertakingIcon()
    {
        undertakingAvailableIcon.gameObject.SetActive(false);
        if (undertakingHolder.undertakings.Count == 0)
            return;
        foreach (var item in undertakingHolder.undertakings)
        {
            if(item.CurrentState == UndertakingState.Inactive)
            {
                undertakingAvailableIcon.gameObject.SetActive(true);
                break;
            }
        }
    }
}
