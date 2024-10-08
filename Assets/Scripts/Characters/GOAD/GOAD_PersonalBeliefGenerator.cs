using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_PersonalBeliefGenerator : MonoBehaviour
	{
        [Serializable]
        public struct Conditions
        {
            public GOAD_ScriptableCondition condition;
            public bool setOnStart;
            public bool setBetweenTimes;
            [ConditionalHide("setBetweenTimes", true)]
            public Vector2 setFromToTimeTick;
            
        }

        [Serializable]
        public class SpecialCondition
        {
            public GOAD_ScriptableCondition condition;
            public string currentActionName;
            public Vector2Int minMaxTimeToSet;
            public CycleTicks cycle;
            public bool canSet;
            public bool isSet;
        }

        [Serializable]
        public class TimedCondition
        {
            public GOAD_ScriptableCondition conditionToSet;
            public List<GOAD_ScriptableCondition> finalPreconditions = new List<GOAD_ScriptableCondition>();
            public bool setDaysAfterConditionsMet;
            [ConditionalHide("setDaysAfterConditionsMet", true)]
            public int totalDaysToWait;
            public List<GOAD_ScriptableCondition> activatePreconditions = new List<GOAD_ScriptableCondition>();
            public int finalDayToBeSet;
            [HideInInspector]
            public bool isSet;

        }

        public List<Conditions> conditions = new List<Conditions>();
        public List<SpecialCondition> specialConditions = new List<SpecialCondition>();
        public List<TimedCondition> timedConditions = new List<TimedCondition>();
        public GOAD_Scheduler scheduler;
        RealTimeDayNightCycle dayNightCycle;

        private void Start()
        {
            foreach (var item in conditions)
            {
                if (item.setOnStart)
                {
                    if (scheduler != null)
                        scheduler.SetBeliefState(item.condition.Condition, item.condition.State);
                }
            }
            GameEventManager.onTimeTickEvent.AddListener(SetBeliefOnTick);
            GameEventManager.onNewDayEvent.AddListener(SetTimedPersonalCondition);
            dayNightCycle = RealTimeDayNightCycle.instance;
        }


        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(SetBeliefOnTick);
            GameEventManager.onNewDayEvent.RemoveListener(SetTimedPersonalCondition);

        }

        void SetBeliefOnTick(int tick)
        {
            SetConditions(tick);
            SetSpecialConditions(tick);
        }

        void SetConditions(int tick)
        {
            if (scheduler == null)
                return;
            foreach (var item in conditions)
            {
                if (!item.setBetweenTimes)
                    continue;

                if (tick >= item.setFromToTimeTick.x && tick < item.setFromToTimeTick.y)
                    scheduler.SetBeliefState(item.condition.Condition, item.condition.State);
                else
                    scheduler.SetBeliefState(item.condition.Condition, !item.condition.State);

            }
        }

        void SetSpecialConditions(int tick)
        {
            foreach (var item in specialConditions)
            {
                if (scheduler.currentActionName != item.currentActionName)
                {
                    item.isSet = false;
                    continue;
                }
                if(item.minMaxTimeToSet == Vector2.zero)
                {
                    scheduler.SetBeliefState(item.condition.Condition, item.condition.State);
                    return;
                }

                    
                if (!item.isSet)
                {
                    int rand = UnityEngine.Random.Range(item.minMaxTimeToSet.x, item.minMaxTimeToSet.y);
                    item.cycle = dayNightCycle.GetCycleTime(rand);
                    item.isSet = true;
                    item.canSet = true;
                }

                if(dayNightCycle.currentTimeRaw >= item.cycle.tick && dayNightCycle.currentDayRaw == item.cycle.day && item.canSet)
                {
                    scheduler.SetBeliefState(item.condition.Condition, item.condition.State);
                    item.canSet = false;
                }

            }
        }

        void SetTimedPersonalCondition(int currentDay)
        {
            var worldStates = GOAD_WorldBeliefStates.instance;
           
            foreach (var condition in timedConditions)
            {
                if (!condition.setDaysAfterConditionsMet)
                {
                    condition.isSet = true;
                    continue;
                }
                    
                if (!condition.isSet)
                {
                    if (scheduler.AreConditionsMet(condition.activatePreconditions))
                    {
                        condition.isSet = true;
                        condition.finalDayToBeSet = dayNightCycle.currentDayRaw + condition.totalDaysToWait;
                    }
                }
                
            }

            
            foreach (var condition in timedConditions)
            {
                if (!condition.isSet)
                    continue;
                if (scheduler.AreConditionsMet(condition.finalPreconditions) && currentDay >= condition.finalDayToBeSet)
                    worldStates.SetWorldState(condition.conditionToSet.Condition, condition.conditionToSet.State);
            } 
            
        }
    }

}