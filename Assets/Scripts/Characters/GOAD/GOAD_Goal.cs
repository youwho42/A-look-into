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

}