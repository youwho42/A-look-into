using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    [Serializable]
    public class SAP_Goal
    {
        public string GoalName;
        public int Priority;
        public List<SAP_Condition> Conditions = new List<SAP_Condition>();
        public SAP_Action Action;
        public bool IsRunning;
        public float TimeLimit;
        public SAP_Condition TimeLimitCondition;
    }
}