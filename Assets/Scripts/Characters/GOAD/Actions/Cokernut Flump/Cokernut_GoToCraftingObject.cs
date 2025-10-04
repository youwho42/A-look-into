using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class Cokernut_GoToCraftingObject : GOAD_Action
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

            if (agent.currentCraftingStation != null)
            {

                agent.currentPathIndex = 0;
                agent.aStarPath.Clear();
                if (agent.StartPositionValid())
                    agent.SetAStarDestination(agent.currentCraftingStation.transform.position, this);
            }
        }

        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);

            if (agent.currentCraftingStation == null)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }



            if (reachedDestination)
            {
                if (!agent.sleep.isSleeping)
                {
                    if (!breaking)
                    {
                        agent.manager.fixSound.transform.position = agent.currentCraftingStation.transform.position;
                        agent.manager.particles.transform.position = agent.currentCraftingStation.transform.position;
                        agent.manager.fixSound.StartSoundsWithTimer();
                        agent.manager.particles.Play();

                        if (agent.currentCraftingStation.transform.position.x < transform.position.x && agent.walker.facingRight)
                            agent.walker.Flip();
                        else if (agent.currentCraftingStation.transform.position.x > transform.position.x && !agent.walker.facingRight)
                            agent.walker.Flip();

                        //agent.currentFixable.StartBreakObject(3f);
                        breaking = true;
                    }
                    agent.animator.SetBool(agent.breakObject_hash, true);
                    breakTimer += Time.deltaTime;
                    if (breakTimer > 3f)
                    {
                        BreakObject(agent);
                        success = true;
                        agent.SetActionComplete(true);
                    }
                }
                else
                {
                    BreakObject(agent);
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


            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreenAStar(this);
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




            if (agent.aStarPath.Count > 0)
                agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck)
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
                    reachedDestination = true;
                    return;
                }

                if (agent.currentPathIndex <= agent.aStarPath.Count - 1)
                    agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];

            }


            agent.walker.SetLastPosition();




        }

        private void BreakObject(GOAD_Scheduler_CF agent)
        {
            foreach (var item in currentItems)
            {
                var offset = Random.insideUnitCircle * Random.Range(0.03f, 0.2f);
                var go = Instantiate(item.Key.ItemPrefabVariants[Random.Range(0, item.Key.ItemPrefabVariants.Count)], agent.currentCraftingStation.transform.position + (Vector3)offset, Quaternion.identity);
                if (go.TryGetComponent(out InteractablePickUp pickup))
                    pickup.pickupQuantity = item.Value;
            }
            Destroy(agent.currentCraftingStation.gameObject);
            agent.animator.SetBool(agent.breakObject_hash, false);
        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
            agent.currentCraftingStation = null;
        }

        Dictionary<QI_ItemData, int> GetAllItemsToSpawn(GOAD_Scheduler_CF agent)
        {
            var item = agent.currentCraftingStation.GetComponent<QI_Item>().Data;
            var inventory = agent.currentCraftingStation.GetComponent<QI_Inventory>();
            var handler = agent.currentCraftingStation.GetComponent<QI_CraftingHandler>();
            var recipes = AllItemsDatabaseManager.instance.allRecipesDatabase.CraftingRecipes;
            Dictionary<QI_ItemData, int> allItems = new Dictionary<QI_ItemData, int>();
            foreach (var recipe in recipes)
            {
                if (recipe.Product.Item != item) continue;
                foreach (var ingredient in recipe.Ingredients)
                {
                    int amount = Mathf.CeilToInt(ingredient.Amount * 0.5f);
                    if (!allItems.ContainsKey(ingredient.Item))
                        allItems.Add(ingredient.Item, amount);
                    else
                        allItems[ingredient.Item] += amount;
                }
                break;
            }

            foreach (var inventoryItem in inventory.Stacks)
            {
                if (!allItems.ContainsKey(inventoryItem.Item))
                    allItems.Add(inventoryItem.Item, inventoryItem.Amount);
                else
                    allItems[inventoryItem.Item] += inventoryItem.Amount;
            }

            if(handler.currentFuel != null)
            {
                if (!allItems.ContainsKey(handler.currentFuel))
                    allItems.Add(handler.currentFuel, handler.currentFuelAmount);
                else
                    allItems[handler.currentFuel] += handler.currentFuelAmount;
            }

            foreach (var queuedItem in handler.Queues)
            {
                foreach (var recipe in recipes)
                {
                    if (recipe.Product.Item != queuedItem.Item) continue;
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        if (!allItems.ContainsKey(ingredient.Item))
                            allItems.Add(ingredient.Item, ingredient.Amount);
                        else
                            allItems[ingredient.Item] += ingredient.Amount;

                    }
                }
            }


            return allItems;
        }

        public Vector3 GetClosestPointOnObjectCollider(GOAD_Scheduler_CF agent, Vector3 position)
        {
            var coll = agent.currentCraftingStation.GetComponentInChildren<PolygonCollider2D>();
            var closestPoint = coll.ClosestPoint(position);
            var dir = ((Vector2)position - closestPoint).normalized * (agent.walker.checkTileDistance + 0.01f);
            return closestPoint + dir;
        }

        public override void ReachFinalDestination(GOAD_Scheduler_CF agent)
        {
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            reachedDestination = true;
        }

    } 
}
