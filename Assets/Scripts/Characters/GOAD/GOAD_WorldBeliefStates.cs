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



        public Dictionary<string, bool> worldStates = new Dictionary<string, bool>();

        public List<InteractableChair> allSeats = new List<InteractableChair>();

        private void Start()
        {
            allSeats.Clear();
            var a = FindObjectsOfType<InteractableChair>().ToList();
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

        }

        public bool HasState(string condition, bool state)
        {
            if (worldStates.TryGetValue(condition, out bool State))
            {
                return State == state;
            }
            return false;
        }

        public InteractableChair FindNearestSeat(Vector3 position)
        {
            float closest = float.MaxValue;
            InteractableChair best = null;
            foreach (var seat in allSeats)
            {
                float d = Vector3.Distance(seat.transform.position, position);
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
