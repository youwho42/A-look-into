using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_StoreHarvestedPlant : GOAD_Action
    {
        bool atBox;
        Vector3 boxPosition;
        bool hasLicked;
        float timer;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.arms.SetActive(false);
            agent.animator.SetBool(agent.walking_hash, true);
            boxPosition = GetSeedBoxPosition(agent);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);

            if (atBox)
            {

                if (!hasLicked)
                {
                    agent.animator.SetBool(agent.walking_hash, false);
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetTrigger(agent.lick_hash);
                    hasLicked = true;
                }

                if (agent.sleep.isSleeping)
                {
                    foreach (var item in agent.plantingArea.seedItem.plantedObject.harvestedItems)
                    {
                        var amount = item.minMaxAmount.x == 0 ? 1 : item.minMaxAmount.x;
                        agent.seedBoxInventory.AddItem(item.harvestedItem, amount, false);
                    }

                    success = true;
                    agent.SetActionComplete(true);
                    return;
                }

                if (timer < 1f)
                    timer += Time.deltaTime;
                else
                {
                    foreach (var item in agent.plantingArea.seedItem.plantedObject.harvestedItems)
                    {
                        var amount = item.minMaxAmount.x == 0 ? 1 : item.minMaxAmount.x;
                        agent.seedBoxInventory.AddItem(item.harvestedItem, amount, false);
                    }

                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }

            if (agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this, boxPosition);
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

            agent.walker.SetWorldDestination(boxPosition);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
                atBox = true;

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            atBox = false;
            hasLicked = false;
            timer = 0;
        }

        Vector3 GetSeedBoxPosition(GOAD_Scheduler_BP agent)
        {
            Vector3 pos = agent.seedBoxInventory.transform.position;
            Vector2 dir = transform.position - pos;
            dir = dir.normalized;
            dir *= 0.12f;
            var colliders = agent.seedBoxInventory.GetComponentsInChildren<Collider2D>();
            foreach (var coll in colliders)
            {
                if (gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    pos = coll.ClosestPoint(transform.position);
                    break;
                }
            }

            return pos + (Vector3)dir;
        }

        public override void ReachFinalDestination(GOAD_Scheduler_BP agent)
        {
            base.ReachFinalDestination(agent);

            atBox = true;
        }
    } 
}
