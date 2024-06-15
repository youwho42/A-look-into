using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Action_FindObject : SAP_Action
    {
        

        public float wanderDistance = 1f;
        bool canGather;
        bool isGathering;
        
        public float gatherAnimTime;
        float timer;

        public QI_ItemDatabase findablesDatabase;
        bool gatheredObject;
        [Range(0.0f, 1.0f)]
        public float gatherChance = 1;
        public override void StartPerformAction(SAP_Scheduler_NPC agent)
        {
            if (target == null)
                target = NavigationNodesManager.instance.GetRandomNode(NavigationNodeType.Outside, agent.pathType, transform.position, 3f);

            gatheredObject = false;
            agent.animator.SetBool(agent.isSitting_hash, false);
            agent.animator.SetBool(agent.isSleeping_hash, false);

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null)
            {
                path.Clear();
                currentPathIndex = 0;
                if (agent.lastValidNode != null)
                    currentNode = agent.lastValidNode;
                else
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, agent.currentNavigationNodeType, agent.pathType);

                path = currentNode.FindPath(target);
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }
        }

        public override void PerformAction(SAP_Scheduler_NPC agent)
        {
            
            if (isGathering)
            {
                timer += Time.deltaTime;
                if (timer >= gatherAnimTime)
                {
                    float gathered = Random.Range(0.0f, 1.0f);
                    if (gathered < gatherChance)
                    {
                        agent.animator.SetTrigger("PickUp");
                        gatheredObject = true;
                        int r = Random.Range(5, 20);
                        agent.agentInventory.AddItem(findablesDatabase.GetRandomWeightedItem(), r, false);
                    }
                    
                    agent.currentGoalComplete = true;
                }
                return;
            }
            if (canGather && !isGathering)
            {
                isGathering = true;
                

                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);


            if (agent.offScreen || agent.sleep.isSleeping)
            {
                agent.HandleOffScreen(this);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (!agent.walker.jumpAhead)
                {
                    if (finalDestination)
                    {
                        ReachFinalDestination(agent);
                        return;
                    }

                    agent.Deviate();
                    return;
                }
            }



            if (path.Count > 0)
            {
                agent.walker.currentDestination = path[currentPathIndex].transform.position;
            }

            //if (!walker.onSlope)
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {
                if (!finalDestination)
                {
                    if (currentPathIndex < path.Count - 1)
                    {
                        currentPathIndex++;
                        currentNode = path[currentPathIndex];
                        agent.walker.currentDestination = path[currentPathIndex].transform.position;
                    }
                    else if (currentPathIndex >= path.Count - 1)
                    {
                        agent.lastValidNode = currentNode;

                        finalDestination = true;
                        agent.walker.SetRandomDestination(wanderDistance);
                    }
                }
                else
                {
                    ReachFinalDestination(agent);
                }

            }

            agent.walker.SetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_NPC agent)
        {

            agent.offScreenPosMoved = true;
            if(gatheredObject)
                agent.SetBeliefState("HasSeed", true);
            timer = 0;
            canGather = false;
            isGathering = false;
            agent.lastValidNode = currentNode;
            currentNode = null;
            path.Clear();
            target = null;
            finalDestination = false;
        }

        

        public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
        {
            agent.offScreenPosMoved = true;
            agent.isDeviating = false;
            canGather = true;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
        }
    }
}
