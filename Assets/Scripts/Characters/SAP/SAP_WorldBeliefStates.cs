using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klaxon.Interactable;
using System;

namespace Klaxon.SAP
{
    public class SAP_WorldBeliefStates : MonoBehaviour
    {
        public static SAP_WorldBeliefStates instance;

        [Serializable]
        public struct Conditions
        {
            public SAP_Condition condition;
            public bool setOnStart;
            public int setOnTimeTick;
            public Vector2 setFromToTimeTick;
        }
        public List<Conditions> conditions = new List<Conditions>();


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public Dictionary<string, bool> worldStates = new Dictionary<string, bool>();

        //public Vector2Int dayTimes;
        //public Vector2Int animalDayTimes;
        //public Vector2Int workTimes;

        public List<InteractableChair> allSeats = new List<InteractableChair>();

        private void Start()
        {
            allSeats.Clear();
            var a = FindObjectsOfType<InteractableChair>().ToList();
            foreach (var seat in a)
            {
                if(seat.sitNode != null)
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

        void SetBeliefOnTick(int tick)
        {
            //bool isDay = false;
            //if (tick >= dayTimes.x && tick < dayTimes.y)
            //    isDay = true;
            //SetWorldState("Day", isDay);

            //bool aniDay = false;
            //if (tick >= animalDayTimes.x && tick < animalDayTimes.y)
            //    aniDay = true;
            //SetWorldState("AnimalDay", aniDay);

            //bool canWork = false;
            //if (tick >= workTimes.x && tick < workTimes.y)
            //    canWork = true;
            //SetWorldState("Work", canWork);
            

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
            
            GameEventManager.onWorldStateUpdateEvent.Invoke();
        }

        public bool HasWorldState(string condition, bool state)
        {
            if (worldStates.ContainsKey(condition))
            {
                if (worldStates[condition] == state)
                    return true;
            }
            return false;
        }

        public bool GetConditionState(string condition)
        {
            if (worldStates.ContainsKey(condition))
            {
                return worldStates[condition];
            }
            return false;
        }

        public InteractableChair FindNearestSeat(Vector3 position)
        {
            float closest = float.MaxValue;
            InteractableChair best = null;
            foreach (var seat in allSeats)
            {
                var d = Vector2.Distance(seat.transform.position, position);
                if(d < closest && seat.canInteract && seat.findNode.active)
                {
                    closest = d;
                    best = seat;
                }
            }
            return best;
        }
    }
}