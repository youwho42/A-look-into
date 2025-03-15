using Klaxon.GOAD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
       
    }

    [Serializable]
    public struct Doot
    {
        public Weekdoots doot;
        public GOAD_ScriptableCondition dootCondition;
    }

    [Serializable]
    public enum Weekdoots
    {
        Soodoot,
        Moodoot,
        Woodoot,
        Toodoot,
        Freedoot
    }
    Weekdoots currentWeekdoot;
    public Weekdoots CurrentWeekdoot { get { return currentWeekdoot; }}
    int dootAmount;
    public List<Doot> doots = new List<Doot>();
    //int startYear = 505;
    //public int currentWeek = 0;
    //int weekAmountPerMonth = 5;
    //public int currentMonth = 0;
    //int monthAmountPerYear = 5;
    //public int currentYear = 0;
    private void Start()
    {
        dootAmount = doots.Count;
        SetDootOfWeek(RealTimeDayNightCycle.instance.currentDayRaw);
        GameEventManager.onNewDayEvent.AddListener(SetDootOfWeek);
        
    }

    private void OnDestroy()
    {
        GameEventManager.onNewDayEvent.RemoveListener(SetDootOfWeek);
    }
    
    public void SetDootOfWeek(int currentDay)
    {
        int day = (currentDay % dootAmount) - 1;
        if (day < 0)
            day = dootAmount - 1;
        
        //currentWeek = Mathf.CeilToInt((float)currentDay / (float)dootAmount);
        //currentMonth = Mathf.CeilToInt((float)currentWeek / (float)weekAmountPerMonth);
        //currentYear = Mathf.FloorToInt((float)currentMonth / (float)monthAmountPerYear) + startYear;
            
        currentWeekdoot = (Weekdoots)day;
        SetWorldBeliefs(currentWeekdoot);
    }
    void SetWorldBeliefs(Weekdoots weekdoot)
    {
        foreach (var doot in doots)
        {
            GOAD_WorldBeliefStates.instance.SetWorldState(doot.dootCondition.Condition, weekdoot == doot.doot);
        }
    }
}
