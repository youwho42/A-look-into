using Klaxon.SAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionallyEnabledWorldItem : MonoBehaviour
{
    [Serializable]
    public struct ConditionalItem
    {
        public GameObject worldItem;
        public SAP_Condition worldCondition;
    }

    public List<ConditionalItem> conditionalItems = new List<ConditionalItem>();
    SAP_WorldBeliefStates worldBeliefStates;

    private void Start()
    {
        worldBeliefStates = SAP_WorldBeliefStates.instance;
        GameEventManager.onWorldStateUpdateEvent.AddListener(ActivateWorldItem);
    }

    public void OnDisable()
    {
        GameEventManager.onWorldStateUpdateEvent.RemoveListener(ActivateWorldItem);
    }

    void ActivateWorldItem()
    {
        foreach (var item in conditionalItems)
        {
            if(worldBeliefStates.HasWorldState(item.worldCondition.Condition, item.worldCondition.State))
                item.worldItem.SetActive(true);

        }
    }
}
