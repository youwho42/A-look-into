using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPWander : GOAD_Action
    {
        public float wanderDistance = 1;
        Vector3 wanderDestination;
        bool isLicking;
        bool hasLicked;
        float timer;

        Vector2 lastDestination;


        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            SetRandomDestinationFrom(agent);
            agent.animator.SetBool(agent.sleeping_hash, false);
            agent.animator.SetBool(agent.walking_hash, true);
            agent.arms.SetActive(false);
            
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
                    success = true;
                    agent.SetActionComplete(true);
                    agent.walker.ResetLastPosition();
                }

                return;
            }



            if (agent.walker.isStuck || agent.isDeviating)
            {
                //if (lastDestination == agent.walker.currentDestination)
                //{
                //    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                //}
                lastDestination = agent.walker.currentDestination;

                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }



            agent.walker.SetWorldDestination(wanderDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                isLicking = true;

            agent.walker.SetLastPosition();
        }

        public override void SucceedAction(GOAD_Scheduler_BP agent)
        {
            base.SucceedAction(agent);
        }

        public override void FailAction(GOAD_Scheduler_BP agent)
        {
            base.FailAction(agent);
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timer = 0;
            isLicking = false;
            hasLicked = false;
        }

        public void SetRandomDestinationFrom(GOAD_Scheduler_BP agent)
        {

            Vector2 rand = Random.insideUnitCircle * wanderDistance;
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