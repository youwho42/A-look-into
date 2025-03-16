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
        bool hasDeviated;
        Vector2 lastDestination;
        float deviateTimer = 0;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            if (!SetRandomDestinationFrom(agent))
            {
                success = false;
                agent.SetActionComplete(true);
            }
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
                lastDestination = agent.walker.currentDestination;
                deviateTimer += Time.deltaTime;
                if(deviateTimer >= 3.0f)
                {
                    success = false;
                    agent.SetActionComplete(true);
                    return;
                }
                if (!agent.walker.jumpAhead)
                {
                    hasDeviated = true;
                    agent.Deviate();
                    return;
                }

            }

            if (hasDeviated)
            {
                wanderDestination = lastDestination;
                hasDeviated = false;
            }
               
            agent.walker.SetWorldDestination(wanderDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                isLicking = true;

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timer = 0;
            isLicking = false;
            hasLicked = false;
            deviateTimer = 0;
        }

        public bool SetRandomDestinationFrom(GOAD_Scheduler_BP agent)
        {

            Vector2 rand = Random.insideUnitCircle * wanderDistance;
            var d = agent.walker.currentTilePosition.groundMap.WorldToCell(new Vector2(agent.BPHomeDestination.x + rand.x, agent.BPHomeDestination.y + rand.y));
            for (int z = agent.walker.currentTilePosition.groundMap.cellBounds.zMax; z > agent.walker.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {
                
                d.z = z;
                if (agent.walker.currentTilePosition.groundMap.GetTile(d) != null)
                {

                    var dif = agent.walker.currentTilePosition.position.z - z;
                    if (Mathf.Abs(dif) > 1)
                        return false;

                    wanderDestination = agent.walker.GetTileWorldPosition(d);
                    wanderDestination += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 1f);
                    var hits = Physics2D.OverlapCircleAll(wanderDestination, 0.05f, LayerMask.GetMask("Obstacle"), wanderDestination.z, wanderDestination.z);
                    
                    return hits.Length <= 0;
                    
                }
            }
            return false;
        }

    }

}