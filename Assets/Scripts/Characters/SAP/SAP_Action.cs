using Klaxon.GravitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action : MonoBehaviour
    {
        
        public NavigationNode target;

        public virtual void StartPerformAction(SAP_Scheduler agent)
        {
        }
        public virtual void PerformAction(SAP_Scheduler agent)
        {
        }
        public virtual void EndPerformAction(SAP_Scheduler agent)
        {
        }
    }
}
