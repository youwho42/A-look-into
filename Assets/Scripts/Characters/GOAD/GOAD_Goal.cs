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
		public GOAD_ScriptableCondition ResultCondition;
        [Header("Pre-Conditions")]
        public GOAD_ScriptableCondition PreCondition;
		
    }

    [Serializable]
    public class GOAD_MainGoal
    {
        [Header("Desired Result")]
        public GOAD_ScriptableCondition ResultCondition;
        [Header("Pre-Conditions")]
        public List<GOAD_ScriptableCondition> PreConditions = new List<GOAD_ScriptableCondition>();

    }

}