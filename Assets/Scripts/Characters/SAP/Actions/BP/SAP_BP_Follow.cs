using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_Follow : SAP_Action
    {

        Vector2 offset;
        Vector2 lastDestination;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            
            offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
        }

        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (agent.walker.isStuck || agent.isDeviating)
            {
                if(lastDestination == agent.walker.currentDestination)
                {
                    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                }
                lastDestination = agent.walker.currentDestination;

                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }
            
            agent.walker.hasDeviatePosition = false;
            if (agent.CheckPlayerDistance() > 3)
            {
                agent.SetBeliefState("PlayerFar", true);
                agent.currentGoalComplete = true;
                return;
            }

            
            agent.walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
           
            agent.walker.SetWorldDestination(agent.walker.currentDestination);
            agent.walker.SetDirection();
            
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                agent.SetBeliefState("PlayerClose", true);
                agent.currentGoalComplete = true;
            }

            agent.walker.SetLastPosition();
        }

        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            agent.justIndicated = false;
            agent.interactor.canInteract = true;

            agent.animator.SetBool(agent.walking_hash, false);
        }

    }
}

