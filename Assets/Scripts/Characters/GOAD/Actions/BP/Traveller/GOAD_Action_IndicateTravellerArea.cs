using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_IndicateTravellerArea : GOAD_Action
    {

        Vector2 offset;
        bool atDestination;
        public InteractableBallPeopleTraveller interactableTraveller;


        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (atDestination)
            {
                
                bool can = CheckUndertakingTasks();
                agent.arms.SetActive(can);
                agent.interactor.canInteract = can;
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
                if (!agent.CheckNearPlayer(2.0f))
                {
                    agent.arms.SetActive(false);
                    agent.justIndicatedTravellerDestination = true;
                    success = false;
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



            agent.walker.SetWorldDestination(agent.BPHomeDestination + (Vector3)offset);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {

                atDestination = true;
            }
            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            atDestination = false;
            agent.hasFoundDestination = false;
            agent.InvokeResetJustIndicatedTravellerDestination();
            base.EndAction(agent);
        }

        bool CheckUndertakingTasks()
        {
            bool tasksComplete = true;
            for (int i = 0; i < interactableTraveller.undertaking.undertaking.Tasks.Count; i++)
            {
                if (interactableTraveller.undertaking.undertaking.Tasks[i] == interactableTraveller.undertaking.task)
                    continue;
                if (!interactableTraveller.undertaking.undertaking.Tasks[i].IsComplete)
                    tasksComplete = false;
            }
            return tasksComplete;
        }

        

    }

}