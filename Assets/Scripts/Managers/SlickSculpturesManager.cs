using System.Collections.Generic;
using UnityEngine;
using Klaxon.GOAD;

public class SlickSculpturesManager : MonoBehaviour
{

    public static SlickSculpturesManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    public List<RestoreSculpture> allSculptures = new List<RestoreSculpture>();
    [HideInInspector]
    public List<RestoreSculpture> sculptureQueue = new List<RestoreSculpture>();




    private void Start()
    {
        GameEventManager.onMuseumPieceUpdateEvent.AddListener(CheckSculptureAvailable);
    }

    private void OnDisable()
    {
        GameEventManager.onMuseumPieceUpdateEvent.RemoveListener(CheckSculptureAvailable);
    }

    void CheckSculptureAvailable()
    {
        foreach (var sculpture in allSculptures)
        {
            if (sculptureQueue.Contains(sculpture))
                continue;
            if(sculpture.hasWorldConditionToFix)
            {
                if(!GOAD_WorldBeliefStates.instance.HasState(sculpture.worldConditionNeeded))
                    continue;
            }
            foreach (var item in sculpture.ingredients)
            {
                if (item.complete && !item.activated)
                {
                    sculptureQueue.Add(sculpture);
                    break;
                }
                    
            }
        }
    }

    public bool HasSculpturesInQueue()
    {
        return sculptureQueue.Count > 0;
    }

    public RestoreSculpture GetNextSculpture()
    {
        if (HasSculpturesInQueue())
            return sculptureQueue[0];
        return null;
    }

    public void RemoveSculptureFromQueue()
    {
        sculptureQueue.RemoveAt(0);
    }

}
