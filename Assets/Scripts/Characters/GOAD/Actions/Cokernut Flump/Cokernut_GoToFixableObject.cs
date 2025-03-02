using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class Cokernut_GoToFixableObject : GOAD_Action
    {

        bool setTargetDestination;
        bool reachedDestination;
        float breakTimer;
        bool breaking;
        Dictionary<QI_ItemData, int> currentItems = new Dictionary<QI_ItemData, int>();

        public override void StartAction(GOAD_Scheduler_CF agent)
        {

            base.StartAction(agent);
            breakTimer = 0;
            breaking = false;
            setTargetDestination = false;
            reachedDestination = false;
            currentItems.Clear();
            currentItems = GetAllItemsToSpawn(agent);
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 0);

            if (agent.currentFixable != null)
            {
                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(agent.currentFixable.transform.position, this);
            }
        }

        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);

            if(agent.currentFixable == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }

            if (reachedDestination)
            {
                if (!breaking)
                {
                    agent.manager.fixSound.transform.position = agent.currentFixable.transform.position;
                    agent.manager.particles.transform.position = agent.currentFixable.transform.position;
                    agent.manager.fixSound.StartSoundsWithTimer();
                    agent.manager.particles.Play();

                    if (agent.currentFixable.transform.position.x < transform.position.x && agent.walker.facingRight)
                        agent.walker.Flip();
                    else if (agent.currentFixable.transform.position.x > transform.position.x && !agent.walker.facingRight)
                        agent.walker.Flip();

                    agent.currentFixable.StartBreakObject(2.5f);
                    breaking = true;
                }
                agent.animator.SetBool(agent.breakObject_hash, true);
                breakTimer += Time.deltaTime;
                if(breakTimer > 3f) 
                {
                    foreach (var item in currentItems)
                    {
                        var offset = Random.insideUnitCircle * Random.Range(0.03f, 0.15f);
                        var go = Instantiate(item.Key.ItemPrefabVariants[Random.Range(0, item.Key.ItemPrefabVariants.Count)], transform.position + (Vector3)offset, Quaternion.identity);
                        if (go.TryGetComponent(out InteractablePickUp pickup))
                            pickup.pickupQuantity = item.Value;
                    }

                    agent.currentFixable = null;
                    agent.animator.SetBool(agent.breakObject_hash, false);
                    success = true;
                    agent.SetActionComplete(true);
                }
                
                return;
            }
            
            if (agent.gettingPath)
                return;

            if (!setTargetDestination)
            {
                agent.currentFinalDestination = GetClosestPointOnObjectCollider(agent, agent.aStarPath.Count == 0 ? transform.position : agent.aStarPath[agent.aStarPath.Count - 1]);
                agent.aStarPath.Add(agent.currentFinalDestination); 
                setTargetDestination = true;
            }

            
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
            {

                if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
                }
                else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                {
                    agent.walker.currentDirection = Vector2.zero;
                    agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
                    agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
                    agent.animator.SetFloat(agent.velocityX_hash, 0);
                    reachedDestination= true;
                    return;
                }

                if (agent.currentPathIndex <= agent.aStarPath.Count - 1)
                    agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];

            }


            agent.walker.SetLastPosition();




        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
            agent.currentFixable = null;
        }

        public Vector3 GetClosestPointOnObjectCollider(GOAD_Scheduler_CF agent, Vector3 position)
        {
            var coll = agent.currentFixable.fixedObject.GetComponentInChildren<PolygonCollider2D>();
            var closestPoint = coll.ClosestPoint(position);
            var dir = ((Vector2)position - closestPoint).normalized * (agent.walker.checkTileDistance + 0.01f);
            return closestPoint + dir;
        }

        Dictionary<QI_ItemData, int> GetAllItemsToSpawn(GOAD_Scheduler_CF agent)
        {
            var ingredients = agent.currentFixable.brokenObject.GetComponent<InteractableFixingArea>().ingredients;
            Dictionary<QI_ItemData, int> allItems = new Dictionary<QI_ItemData, int>();
            foreach (var item in ingredients)
            {
                int amount = Mathf.CeilToInt(item.amount * 0.5f);
                if (!allItems.ContainsKey(item.item))
                    allItems.Add(item.item, amount);
                else
                    allItems[item.item] += amount;
            }

            


            return allItems;
        }

        public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_NPC agent)
        {
            Debug.Log("Already there");
            //success = true;
            //agent.SetActionComplete(true);
        }

    }
}
