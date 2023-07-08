using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_IndicateTravellerArea : SAP_Action
    {

        Vector2 offset;
        bool atDestination;
        public InteractableBallPeopleTraveller interactableTraveller;


        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        }

        public override void PerformAction(SAP_Scheduler_BP agent)
        {

            if (atDestination)
            {
                bool can = CheckUndertakingTasks();
                agent.arms.SetActive(can);
                agent.interactor.canInteract = can;
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDir = Vector2.zero;
                if (agent.CheckPlayerDistance() > 1.5f)
                {
                    agent.arms.SetActive(false);
                    agent.justIndicated = true;
                    agent.SetBeliefState("TravellerDestinationFound", false);
                    agent.SetBeliefState("IsHome", true);
                    agent.currentGoalComplete = true;
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



            agent.walker.SetWorldDestination(agent.travellerDestination + (Vector3)offset);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                
                atDestination = true;
            }
            agent.walker.SetLastPosition();
        }

        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = true;
            atDestination = false;
            agent.hasFoundDestination = false;
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

