using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_PersonalBeliefGenerator : MonoBehaviour
	{
        [Serializable]
        public struct Conditions
        {
            public GOAD_Condition condition;
            public bool setOnStart;
            public bool setBetweenTimes;
            [ConditionalHide("setBetweenTimes", true)]
            public Vector2 setFromToTimeTick;
            
        }

        [Serializable]
        public class SpecialCondition
        {
            public GOAD_Condition condition;
            public string currentActionName;
            public Vector2Int minMaxTimeToSet;
            public CycleTicks cycle;
            public bool canSet;
            public bool isSet;
        }

        public List<Conditions> conditions = new List<Conditions>();
        public List<SpecialCondition> specialConditions = new List<SpecialCondition>();
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
            dayNightCycle = RealTimeDayNightCycle.instance;
        }


        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(SetBeliefOnTick);
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
    }

}