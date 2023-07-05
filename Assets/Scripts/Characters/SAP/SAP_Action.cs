using Klaxon.GravitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action : MonoBehaviour
    {
        /// <summary>
        /// NPC Actions
        /// </summary>
        public NavigationNode target;
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
    }
}
