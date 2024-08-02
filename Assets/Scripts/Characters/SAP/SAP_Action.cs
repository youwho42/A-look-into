using Klaxon.GravitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action : MonoBehaviour
    {
        
        [HideInInspector]
        public List<NavigationNode> path = new List<NavigationNode>();
        [HideInInspector]
        public int currentPathIndex;
        [HideInInspector]
        public NavigationNode currentNode;
        [HideInInspector]
        public bool finalDestination;


        /// <summary>
        /// NPC Actions
        /// </summary>
        public string attachedGoal = "Please set this for clarity";
        public NavigationNode target;
        public bool setConditionOnComplete;
        [ConditionalHide("setConditionOnComplete", true)]
        public SAP_Condition conditionToSet;
        public virtual void InitialCheckPerformAction(SAP_Scheduler_NPC agent)
        {
        }
        public virtual void StartPerformAction(SAP_Scheduler_NPC agent)
        {
        }
        public virtual void PerformAction(SAP_Scheduler_NPC agent)
        {
        }
        public virtual void EndPerformAction(SAP_Scheduler_NPC agent)
        {
        }
        public virtual void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
        }

        /// <summary>
        /// BP Actions
        /// </summary>
        public virtual void InitialCheckPerformAction(SAP_Scheduler_BP agent)
        {
        }
        public virtual void StartPerformAction(SAP_Scheduler_BP agent)
        {
        }
        public virtual void PerformAction(SAP_Scheduler_BP agent)
        {
        }
        public virtual void EndPerformAction(SAP_Scheduler_BP agent)
        {
        }
        public virtual void ReachFinalDestination(SAP_Scheduler_BP agent)
        {
        }

        /// <summary>
        /// ANIMAL Actions
        /// </summary>
        public virtual void InitialCheckPerformAction(SAP_Scheduler_ANIMAL agent)
        {
        }
        public virtual void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
        }
        public virtual void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
        }
        public virtual void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
        }
    }
}
