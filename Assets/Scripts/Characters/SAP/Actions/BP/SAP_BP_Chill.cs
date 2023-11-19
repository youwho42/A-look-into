using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_Chill : SAP_Action
    {
        float timeIdle;
        bool sleeping;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = true;

            agent.walker.currentDir = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.ResetLastPosition();
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (!sleeping)
            {
                
                var dir = PlayerInformation.instance.player.position - transform.position;
                agent.walker.SetFacingDirection(dir);
                agent.arms.SetActive(!agent.hasInteracted);
                agent.interactor.canInteract = !agent.hasInteracted;
            }
            

          

            timeIdle += Time.deltaTime;
            if (agent.CheckPlayerDistance() > 3)
            {
                
                agent.SetBeliefState("PlayerFar", true);
                agent.currentGoalComplete = true;
                return;
            }

            if (timeIdle > 0.5f)
            {
                if (agent.CheckPlayerDistance() >= .5f)
                {
                    //interactor.canInteract = false;
                    agent.SetBeliefState("PlayerClose", false);
                    agent.currentGoalComplete = true;
                    return;
                }

            }

            if(sleeping)
            {
                agent.interactor.canInteract = false;

                agent.animator.SetBool(agent.sleeping_hash, true);
                return;
            }
            if (timeIdle >= 10.0f)
            {
                timeIdle = 0;
                sleeping = true;
            }
            
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            agent.animator.SetBool(agent.sleeping_hash, false);
            timeIdle = 0;
            sleeping = false;
            agent.interactor.canInteract = true;

        }
    }

}
