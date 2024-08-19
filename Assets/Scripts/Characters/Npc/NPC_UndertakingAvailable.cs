using Klaxon.GOAD;
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
            if (item.Undertaking.CurrentState == UndertakingState.Active)
            {
                break;
            }
            if (item.Undertaking.CurrentState == UndertakingState.Inactive)
            {
                if (item.hasCondition)
                {
                    if(GOAD_WorldBeliefStates.instance.HasState(item.UndertakingAvailableCondition.Condition, item.UndertakingAvailableCondition.State))
                    {
                        undertakingAvailableIcon.gameObject.SetActive(true);
                        break;
                    }
                    else
                        break;
                        
                }
                undertakingAvailableIcon.gameObject.SetActive(true);
                break;
            }
        }
    }

    
}
