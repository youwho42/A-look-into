using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_WorldBeliefStates : MonoBehaviour
    {
        public static SAP_WorldBeliefStates instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public Dictionary<string, bool> worldStates = new Dictionary<string, bool>();

        public Vector2Int dayTimes;
        public Vector2Int animalDayTimes;
        public Vector2Int workTimes;

        public List<InteractableChair> allSeats = new List<InteractableChair>();

        private void Start()
        {
            allSeats.Clear();
            var a = FindObjectsOfType<InteractableChair>().ToList();
            foreach (var seat in a)
            {
                allSeats.Add(seat);
            }
        }
        private void OnEnable()
        {
            GameEventManager.onTimeTickEvent.AddListener(SetDayBelief);
        }

        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(SetDayBelief);
        }

        void SetDayBelief(int tick)
        {
            bool isDay = false;
            if (tick >= dayTimes.x && tick < dayTimes.y)
                isDay = true;
            SetWorldState("Day", isDay);

            bool aniDay = false;
            if (tick >= animalDayTimes.x && tick < animalDayTimes.y)
                aniDay = true;
            SetWorldState("AnimalDay", aniDay);

            bool canWork = false;
            if (tick >= workTimes.x && tick < workTimes.y)
                canWork = true;
            SetWorldState("Work", canWork);
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

        public InteractableChair FindNearestSeat(Vector3 position)
        {
            float closest = float.MaxValue;
            InteractableChair best = null;
            foreach (var seat in allSeats)
            {
                var d = Vector2.Distance(seat.transform.position, position);
                if(d < closest && seat.canInteract && seat.navigationNode.active)
                {
                    closest = d;
                    best = seat;
                }
            }
            return best;
        }
    }
}