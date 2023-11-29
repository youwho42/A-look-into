using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_GetSeedFromBox : SAP_Action
    {
        bool atBox;
        Vector3 boxPosition;
        bool hasLicked;
        float timer;
        bool resetBoxPosition;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.walking_hash, true);
            boxPosition = GetSeedBoxPosition(agent);
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {

            if (atBox)
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
                    agent.seedBoxInventory.RemoveItem(agent.plantingArea.seedItem, 1);
                    agent.SetBeliefState("HasSeed", true);
                    agent.SetBeliefState("SeedAvailable", false);
                    agent.currentGoalComplete = true;
                }
                return;
            }


            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    resetBoxPosition = true;
                    return;
                }
            }

            if (resetBoxPosition)
            {
                boxPosition = GetSeedBoxPosition(agent);
                resetBoxPosition = false;
            }
                
            agent.walker.hasDeviatePosition = false;



            agent.walker.SetWorldDestination(boxPosition);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                atBox = true;

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            atBox = false;
            hasLicked = false;
            timer = 0;
        }

        Vector3 GetSeedBoxPosition(SAP_Scheduler_BP agent)
        {
            Vector3 pos = agent.seedBoxInventory.transform.position;
            Vector2 dir = transform.position - pos;
            dir = dir.normalized;
            dir *= 0.12f;
            var colliders = agent.seedBoxInventory.GetComponentsInChildren<Collider2D>();
            foreach (var coll in colliders)
            {
                if (gameObject.layer == agent.obstacleLayer)
                {
                    pos = coll.ClosestPoint(transform.position);
                    break;
                }
            }

            return pos + (Vector3)dir;
        }


        
    }
}

