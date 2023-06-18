using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;

namespace Klaxon.GOAP
{
	public abstract class GOAP_Action : MonoBehaviour
	{
		public string actionName = "Action";
		public float cost = 1.0f;
		
		public NavigationNode target;
		public Vector2Int minMaxDuration;
		public int fixedTime;
		
		public WorldState[] preConditions;
		public WorldState[] afterEffects;
		[HideInInspector]
		public GravityItemWalker walker;

		public Dictionary<string, int> preconditions;
		public Dictionary<string, int> effects;

		public GOAP_WorldStates agentBeliefs;

		public bool running = false;

		public GOAP_Action()
		{
			preconditions= new Dictionary<string, int>();
			effects= new Dictionary<string, int>();
		}

        public void Awake()
        {
            walker = gameObject.GetComponent<GravityItemWalker>();
			if(preConditions != null)
			{
				foreach (var c in preConditions)
				{
					preconditions.Add(c.key, c.value);
				}
			}
            if (afterEffects != null)
            {
                foreach (var e in afterEffects)
                {
                    effects.Add(e.key, e.value);
                }
            }
			agentBeliefs = GetComponent<GOAP_Agent>().beliefs;
        }

		public bool IsAchievable()
		{
			return true;
		}
		

		public bool IsAchievableGiven(Dictionary<string, int> conditions)
		{
			foreach (var c in preconditions)
			{
				if(!conditions.ContainsKey(c.Key))
					return false;
			}
			return true;
		}
		public int GetFinalDuration()
		{
			int t = RealTimeDayNightCycle.instance.currentTimeRaw;
			if (fixedTime != 0)
			{
				if (fixedTime > t)
					return fixedTime - t;
				else
					return 1440 - (t - fixedTime);
			}
				 
			int finalX = 1;
            if (minMaxDuration.x != 0)
                finalX = minMaxDuration.x;
            if (minMaxDuration.y == 0)
				return finalX;
                
				
			return Random.Range(finalX, minMaxDuration.y);
		}
		public abstract bool PrePerform(GOAP_Agent agent);
		public abstract void Perform(GOAP_Agent agent);
		public abstract void PrePostPerform(GOAP_Agent agent);
		public abstract bool PostPerform(GOAP_Agent agent);

    } 
}
