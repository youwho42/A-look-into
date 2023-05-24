using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.FSM
{
	public abstract class FSM_Decision : ScriptableObject
	{
		public abstract void ResetDecision(FSM_Brain brain);
		public abstract bool Decide(FSM_Brain brain);
		
	} 
}
