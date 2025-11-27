using Klaxon.Interactable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_WorldBeliefStates : MonoBehaviour
	{
		public static GOAD_WorldBeliefStates instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        [Serializable]
        public struct Conditions
        {
            public string conditionName;
            public GOAD_ScriptableCondition condition;
            public bool setOnStart;
            public int setOnTimeTick;
            public Vector2 setFromToTimeTick;
        }
        public List<Conditions> conditions = new List<Conditions>();
        [Serializable]
        public class ConditionalCondition
        {
            public string conditionName;
            public GOAD_ScriptableCondition conditionNeeded;
            public GOAD_ScriptableCondition conditionSet;
            public bool setDaysAfterConditionsMet;
            [ConditionalHide("setDaysAfterConditionsMet", true)]
            public int totalDaysToWait;
            public int finalDayToBeSet;
            [HideInInspector]
            public bool isReady;
            [HideInInspector]
            public bool isSet;
        }
        public List<ConditionalCondition> conditionalConditions = new List<ConditionalCondition>();

        [Serializable]
        public struct RepeatableConditionalCondition
        {
            public string conditionName;
            public GOAD_ScriptableCondition conditionToSet;
            public List<GOAD_ScriptableCondition> neededConditions;
            public Vector2 setFromToTimeTick;
        }
        public List<RepeatableConditionalCondition> repeatableConditions = new List<RepeatableConditionalCondition>();

        public Dictionary<string, bool> worldStates = new Dictionary<string, bool>();

        public List<InteractableChair> allSeats = new List<InteractableChair>();

        private void Start()
        {
            allSeats.Clear();
            var a = FindObjectsByType<InteractableChair>(FindObjectsSortMode.None).ToList();
            foreach (var seat in a)
            {
                if (seat == null)
                    continue;
                if (seat.sitNode != null)
                {
                    if (!seat.isPrivateSeat)
                        allSeats.Add(seat);
                }

            }

            foreach (var item in conditions)
            {
                if (item.setOnStart)
                {
                    SetWorldState(item.condition.Condition, item.condition.State);

                }

            }


        }

        private void OnEnable()
        {
            GameEventManager.onTimeTickEvent.AddListener(SetBeliefOnTick);
        }

        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(SetBeliefOnTick);
        }


        public void SetWorldState(string condition, bool state)
        {
            if (!worldStates.ContainsKey(condition))
            {
                worldStates.Add(condition, state);
            }
            else
            {
                worldStates[condition] = state;
            }
            SetConditionalConditions();
            GameEventManager.onWorldStateUpdateEvent.Invoke();
        }


        void SetBeliefOnTick(int tick)
        {
            
            foreach (var item in conditions)
            {
                if (item.setOnTimeTick != 0)
                {
                    if (tick == item.setOnTimeTick)
                    {
                        SetWorldState(item.condition.Condition, item.condition.State);
                    }

                }
                if (item.setFromToTimeTick != Vector2.zero)
                {
                    SetWorldState(item.condition.Condition, !item.condition.State);
                    if (tick >= item.setFromToTimeTick.x && tick < item.setFromToTimeTick.y)
                        SetWorldState(item.condition.Condition, item.condition.State);
                }
            }
            SetRepeatableConditionalConditions();
        }

        void SetConditionalConditions()
        {
            foreach (var condition in conditionalConditions)
            {
                if (condition.isSet)
                    continue;
                
                if (HasState(condition.conditionNeeded.Condition, condition.conditionNeeded.State))
                {
                    if (!condition.isReady)
                    {
                        condition.finalDayToBeSet = RealTimeDayNightCycle.instance.currentDayRaw + condition.totalDaysToWait;
                        condition.isReady = true;
                    }
                    if(condition.isReady && RealTimeDayNightCycle.instance.currentDayRaw == condition.finalDayToBeSet)
                    {
                        if (!worldStates.ContainsKey(condition.conditionSet.Condition))
                            worldStates.Add(condition.conditionSet.Condition, condition.conditionSet.State);
                        else
                            worldStates[condition.conditionSet.Condition] = condition.conditionSet.State;

                        condition.isSet = true;
                    }
                }
            }
            
        }

        void SetRepeatableConditionalConditions()
        {
            int timeTick = RealTimeDayNightCycle.instance.currentTimeRaw;
            foreach (var condition in repeatableConditions)
            {
                bool canSet = timeTick > condition.setFromToTimeTick.x && timeTick < condition.setFromToTimeTick.y;
                
                foreach (var c in condition.neededConditions)
                {
                    if (!HasState(c.Condition, c.State))
                    {
                        canSet = false;
                        break;
                    }

                }
                
                if (!worldStates.ContainsKey(condition.conditionToSet.Condition))
                    worldStates.Add(condition.conditionToSet.Condition, canSet ? condition.conditionToSet.State : !condition.conditionToSet.State);
                else
                    worldStates[condition.conditionToSet.Condition] = canSet ? condition.conditionToSet.State : !condition.conditionToSet.State;
                
            }

        }

        public bool HasState(string condition, bool state)
        {
            if (worldStates.TryGetValue(condition, out bool State))
            {
                return State == state;
            }
            return false;
        }

        public bool HasState(GOAD_ScriptableCondition condition)
        {
            if (worldStates.TryGetValue(condition.Condition, out bool State))
            {
                return State == condition.State;
            }
            return false;
        }

        public bool HasStates(List<GOAD_ScriptableCondition> conditions)
        {
            bool hasAllStates = true;
            foreach (var condition in conditions)
            {
                if (!HasState(condition))
                {
                    hasAllStates = false; 
                    break;
                }
            }
            
            return hasAllStates;
        }

        public InteractableChair FindNearestSeat(Vector3 position)
        {
            float closest = float.MaxValue;
            InteractableChair best = null;
            foreach (var seat in allSeats)
            {
                float d = NumberFunctions.GetDistanceV3(seat.transform.position, position);
                if (d < closest && seat.canInteract)
                {
                    closest = d;
                    best = seat;
                }
            }
            
            
            return best;
        }

    } 
}
