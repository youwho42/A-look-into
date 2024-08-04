using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action : MonoBehaviour
	{
        public string actionName;
		public GOAD_Goal Goal;
        public bool IsRunning;
        public bool success;
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
        }
        public virtual void FailAction(GOAD_Scheduler_NPC agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
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
    } 
}
