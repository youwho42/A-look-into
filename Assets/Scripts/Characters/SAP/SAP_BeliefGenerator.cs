using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    
    public class SAP_BeliefGenerator : MonoBehaviour
    {
        [Serializable]
        public struct Conditions
        {
            public SAP_Condition condition;
            public bool setOnStart;
            public int setOnTimeTick;
        }
        public List<Conditions> conditions = new List<Conditions>();
        public SAP_Scheduler_NPC schedulerNPC;
        public SAP_Scheduler_BP schedulerBP;
        private void Start()
        {
            foreach (var item in conditions)
            {
                if (item.setOnStart)
                {
                    if (schedulerNPC != null)
                        schedulerNPC.SetBeliefState(item.condition.Condition, item.condition.State);
                    if (schedulerBP != null)
                        schedulerBP.SetBeliefState(item.condition.Condition, item.condition.State);
                }

            }

            GameEventManager.onTimeTickEvent.AddListener(SetBeliefOnTick);
        }
        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(SetBeliefOnTick);
        }

        void SetBeliefOnTick(int tick)
        {
            foreach (var item in conditions)
            {
                if (item.setOnTimeTick != 0)
                {
                    if(item.setOnTimeTick == tick)
                    {
                        if (schedulerNPC != null)
                            schedulerNPC.SetBeliefState(item.condition.Condition, item.condition.State);
                        if (schedulerBP != null)
                            schedulerBP.SetBeliefState(item.condition.Condition, item.condition.State);
                    }
                    
                }
            }
        }
    }
}

