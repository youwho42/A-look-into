using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPSeekItem : GOAD_Action
    {

        bool isLicking;
        bool hasLicked;
        float timer;


        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (isLicking)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
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
                    if (agent.currentSeekItem != null)
                        agent.seekItemsFound.Add(agent.currentSeekItem.transform.position);
                    agent.currentSeekItem = null;
                    agent.isSeeking = false;
                    success = true;
                    agent.SetActionComplete(true);
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

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timer = 0;
            hasLicked = false;
            isLicking = false;
            agent.interactor.canInteract = true;
        }
    }

}