using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	[Serializable]
	public class GOAD_Goal
	{
		[Header("Desired Result")]
		public GOAD_Condition ResultCondition;
        [Header("Pre-Conditions")]
        public GOAD_Condition PreCondition;
		
    }

    [Serializable]
    public class GOAD_MainGoal
    {
        [Header("Desired Result")]
        public GOAD_Condition ResultCondition;
        [Header("Pre-Conditions")]
        public List<GOAD_Condition> PreConditions = new List<GOAD_Condition>();

    }

}