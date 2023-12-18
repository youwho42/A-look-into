using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_Rest : SAP_Action
	{
        public Vector2 headTimeRange;
        float headTimer;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isSitting_hash, true);
            headTimer = agent.SetRandomRange(headTimeRange);
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {


            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.isSitting_hash, true);

            headTimer -= Time.deltaTime;
            if (headTimer <= 0)
                TurnHead(agent);
            

        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            headTimer = 0;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.SetBeliefState("Rest", false);
        }


        void TurnHead(SAP_Scheduler_ANIMAL agent)
        {

            headTimer = agent.SetRandomRange(headTimeRange);
            agent.animator.SetTrigger(agent.idle_hash);

        }
    }

}