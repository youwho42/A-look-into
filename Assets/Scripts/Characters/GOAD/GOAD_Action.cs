using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action : MonoBehaviour
	{
        public string actionName;
		public GOAD_Goal Goal;
        [HideInInspector]
        public bool IsRunning;
        [HideInInspector]
        public bool success;
        public List<GOAD_ScriptableCondition> conditionsOnSucceed = new List<GOAD_ScriptableCondition>();
        public List<GOAD_ScriptableCondition> conditionsOnFail = new List<GOAD_ScriptableCondition>();

        public virtual void StartAction(GOAD_Scheduler_NPC agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_NPC agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_NPC agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_NPC agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_NPC agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        public virtual void AStarDestinationIsCurrentPosition(GOAD_Scheduler_NPC agent)
        {

        }

        public virtual void OffscreenNodeHandleComplete(GOAD_Scheduler_NPC agent)
        {

        }
        
    } 
}