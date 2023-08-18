using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_SeekItem : SAP_Action
    {
        bool isLicking;
        bool hasLicked;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
        }

        public override void PerformAction(SAP_Scheduler_BP agent)
        {

            if (isLicking)
            {
                
                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDir = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }
                
                
                if (timer < 1f)
                    timer += Time.deltaTime;
                else
                {
                    agent.foundAmount++;
                    if (agent.foundAmount == agent.seekAmount)
                    {
                        agent.task.undertaking.TryCompleteTask(agent.task.task);
                        agent.hasInteracted = false;
                    }
                    agent.seekItemsFound.Add(agent.currentSeekItem.transform.position);
                    agent.currentSeekItem = null;
                    agent.currentGoalComplete = true;
                    agent.SetBeliefState("SeekItemFound", false);
                    agent.isSeeking = false;
                    
                }
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;


            
            agent.walker.SetWorldDestination(agent.currentSeekItemLocation);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                isLicking = true;
            }
            agent.walker.SetLastPosition();
        }

        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timer = 0;
            hasLicked = false;
            isLicking = false;
            agent.interactor.canInteract = true;

        }



    }

}
