using Klaxon.GOAD;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnQuestComplete : MonoBehaviour
{
    public List<GameObject> objectsToActivate = new List<GameObject>();
    public string undertakingName;
    private void Start()
    {
        GameEventManager.onUndertakingsUpdateEvent.AddListener(SetObjectsToActivate);
        SetObjectsToActivate();
    }
    private void OnDisable()
    {
        GameEventManager.onUndertakingsUpdateEvent.RemoveListener(SetObjectsToActivate);
    }
    public void SetObjectsToActivate()
    {
        bool active = GOAD_WorldBeliefStates.instance.HasState(undertakingName, true);
        foreach (var item in objectsToActivate)
        {
            item.SetActive(active);
        }
        
        
    }
}
