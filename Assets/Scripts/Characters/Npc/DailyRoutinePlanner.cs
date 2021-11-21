using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutinePlanner : MonoBehaviour
{
    public enum Locations
    {
        Home,
        Work
    }
    [Serializable]
    public struct DailyLocation
    {
        public Locations location;
        public int nodeDestinationID;
        public List<int> goToLocationTime;
    }

    public List<DailyLocation> dailyLocations = new List<DailyLocation>();

    DayNightCycle dayNightCycle;
    PathfindingGoToNode pathfinding;
    private void Start()
    {
        pathfinding = GetComponent<PathfindingGoToNode>();
        dayNightCycle = DayNightCycle.instance;
        dayNightCycle.FullHourEventCallBack.AddListener(SetRoutine);
    }

    void SetRoutine(int time)
    {
        
        foreach (var loc in dailyLocations)
        {
            if (loc.goToLocationTime.Contains(time))
                pathfinding.SetDestination(loc.nodeDestinationID);
        }
    }

    
}


