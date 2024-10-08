using Klaxon.GOAD;
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
        public GOAD_ScriptableCondition worldCondition;
        public bool activate;
    }

    public List<ConditionalItem> conditionalItems = new List<ConditionalItem>();
    GOAD_WorldBeliefStates worldBeliefStates;

    private void Start()
    {
        worldBeliefStates = GOAD_WorldBeliefStates.instance;
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
            if(worldBeliefStates.HasState(item.worldCondition.Condition, item.worldCondition.State))
                item.worldItem.SetActive(item.activate);

        }
    }
}
