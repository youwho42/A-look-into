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


        /// <summary>
        /// NPC GOAD
        /// </summary>
        
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



        /// <summary>
        /// BP GOAD
        /// </summary>
        /// 
        public virtual void StartAction(GOAD_Scheduler_BP agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_BP agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_BP agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_BP agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_BP agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        public virtual void ReachFinalDestination(GOAD_Scheduler_BP agent)
        {
            
        }






        /// <summary>
        /// ANIMAL GOAD
        /// </summary>
        /// 
        public virtual void StartAction(GOAD_Scheduler_Animal agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_Animal agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_Animal agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_Animal agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_Animal agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        /// <summary>
        /// Ghost GOAD
        /// </summary>

        public virtual void StartAction(GOAD_Scheduler_Ghost agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_Ghost agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_Ghost agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_Ghost agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_Ghost agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        public virtual void AStarDestinationIsCurrentPosition(GOAD_Scheduler_Ghost agent)
        {
        }


        /// <summary>
        /// ROBOT GOAD
        /// </summary>

        public virtual void StartAction(GOAD_Scheduler_Robot agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_Robot agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_Robot agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_Robot agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_Robot agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        public virtual void AStarDestinationIsCurrentPosition(GOAD_Scheduler_Robot agent)
        {
        }

        public virtual void OffscreenNodeHandleComplete(GOAD_Scheduler_Robot agent)
        {
        }




        /// <summary>
        /// Cokernut Flump GOAD
        /// </summary>

        public virtual void StartAction(GOAD_Scheduler_CF agent)
        {
            IsRunning = true;
            success = false;
        }
        public virtual void PerformAction(GOAD_Scheduler_CF agent)
        {
        }
        public virtual void SucceedAction(GOAD_Scheduler_CF agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, Goal.ResultCondition.State);
            foreach (var condition in conditionsOnSucceed)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }
        public virtual void FailAction(GOAD_Scheduler_CF agent)
        {
            agent.SetBeliefState(Goal.ResultCondition.Condition, !Goal.ResultCondition.State);
            foreach (var condition in conditionsOnFail)
            {
                agent.SetBeliefState(condition.Condition, condition.State);
            }
        }

        public virtual void EndAction(GOAD_Scheduler_CF agent)
        {
            IsRunning = false;
            if (success)
                SucceedAction(agent);
            else
                FailAction(agent);
            success = false;
        }

        public virtual void AStarDestinationIsCurrentPosition(GOAD_Scheduler_CF agent)
        {
        }

        public virtual void ReachFinalDestination(GOAD_Scheduler_CF agent)
        {
        }

    }
}
