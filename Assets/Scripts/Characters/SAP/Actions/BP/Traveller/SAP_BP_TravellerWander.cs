using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_TravellerWander : SAP_Action
    {

        Vector3 wanderDestination;
        bool isLicking;
        bool hasLicked;
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            SetRandomDestinationFrom(agent);
            agent.animator.SetBool(agent.sleeping_hash, false);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.SetBeliefState("IsHome", false);
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
                    agent.currentGoalComplete = true;
                   
                return;
            }



            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.walker.hasDeviatePosition = false;
                    agent.isDeviating = false;
                    agent.currentGoalComplete = true;
                    agent.walker.currentDir = Vector2.zero;
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.ResetLastPosition();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;

            agent.walker.SetWorldDestination(wanderDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                isLicking = true;
            
            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timer = 0;
            isLicking = false;
            hasLicked = false;

        }

        public void SetRandomDestinationFrom(SAP_Scheduler_BP agent)
        {

            Vector2 rand = Random.insideUnitCircle * agent.seekRadius;
            var d = agent.walker.currentTilePosition.groundMap.WorldToCell(new Vector2(agent.travellerDestination.x + rand.x, agent.travellerDestination.y + rand.y));
            for (int z = agent.walker.currentTilePosition.groundMap.cellBounds.zMax; z > agent.walker.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {
                d.z = z;
                if (agent.walker.currentTilePosition.groundMap.GetTile(d) != null)
                {

                    wanderDestination = agent.walker.GetTileWorldPosition(d);
                    wanderDestination += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                    break;
                }
            }
        }

    }
}

