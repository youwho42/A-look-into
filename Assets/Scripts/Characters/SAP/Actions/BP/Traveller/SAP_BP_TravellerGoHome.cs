using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.SAP
{
    public class SAP_BP_TravellerGoHome : SAP_Action
    {
        Vector2 offset;
        float timeDeviating;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.animator.SetBool(agent.walking_hash, true);
            offset = new Vector2(Random.Range(-0.1f, 0.1f), -0.15f);
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    timeDeviating += Time.deltaTime;
                    if (timeDeviating >= 15)
                    {
                        transform.position = agent.travellerDestination + (Vector3)offset;
                        agent.walker.SetLastPosition();
                    }
                        
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;
            


            agent.walker.SetWorldDestination(agent.travellerDestination + (Vector3)offset);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                agent.SetBeliefState("IsHome", true);
                agent.currentGoalComplete = true;
            }
            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timeDeviating = 0;
        }
    }
}

