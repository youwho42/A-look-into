using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_UndertakingAvailable : MonoBehaviour
{
    UndertakingHolder undertakingHolder;
    public SpriteRenderer undertakingAvailableIcon;
    public bool isInactive;

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


    public void SetUndertakingIcon()
    {
        undertakingAvailableIcon.gameObject.SetActive(false);
        if (undertakingHolder == null)
            return;
        if (undertakingHolder.undertakings.Count == 0 || isInactive)
            return;
        foreach (var item in undertakingHolder.undertakings)
        {
            if (item.CurrentState == UndertakingState.Active)
            {
                break;
            }
            if (item.CurrentState == UndertakingState.Inactive)
            {
                undertakingAvailableIcon.gameObject.SetActive(true);
                break;
            }
        }
    }

    
}
