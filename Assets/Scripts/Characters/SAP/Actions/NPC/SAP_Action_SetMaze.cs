using Klaxon.MazeTech;
using Klaxon.SAP;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAP_Action_SetMaze : SAP_Action
{
    SAP_WorldBeliefStates worldStates;

    bool destinationReached;
    public QI_ItemData mazeTicket;
    float positionTimer;
    public Vector2 positionTimerMinMax;
    public MazeCreator mazeCreator;
    public ParticleSystem setupParticles;
    bool particlesOn;
    public override void StartPerformAction(SAP_Scheduler_NPC agent)
    {
        particlesOn = false;
        worldStates = SAP_WorldBeliefStates.instance;
        agent.animator.SetBool(agent.isSitting_hash, false);
        agent.animator.SetBool(agent.isSleeping_hash, false);
        if (target != null)
        {

            // Set the destination (currentAction.target) and direction here using currentAction.walker
            if (currentNode == null && path.Count <= 0)
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
        
        setupParticles.Stop();
        positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
    }

    public override void PerformAction(SAP_Scheduler_NPC agent)
    {

        if (agent.HasBelief("ResetMaze", false))
        {
            setupParticles.Stop();
            mazeCreator.StartMazeCreation();
            worldStates.SetWorldState("MazeOpen", true);
            agent.SetBeliefState("CanSell", true);
            transform.position = target.transform.position;
            agent.agentInventory.AddItem(mazeTicket, 1, false);
            agent.currentGoalComplete = true;
            
        }


        if (destinationReached)
        {
            if (!particlesOn)
            {
                setupParticles.Play();
                particlesOn = true;
            }
            
            positionTimer -= Time.deltaTime;
            if(positionTimer < 0)
            {
                positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
                int r = Random.Range(0, mazeCreator.allTiles.Count);
                Vector3 randomTile = mazeCreator.allTiles[r].TileCenter;
                randomTile.z = transform.position.z;
                transform.position = randomTile;
            }

            

            return;
        }

        


        // we're getting to the main position, at the end of this we start actually working on the maze
        agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
        agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
        // this is where we need to make the npc GO TO the destination.
        // use currentAction.walker here


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
                agent.Deviate();
                return;
            }
        }

        if (path.Count > 0)
        {
            agent.walker.currentDestination = path[currentPathIndex].transform.position;
        }

        if (!agent.walker.onSlope)
            agent.walker.SetDirection();
        if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
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

                ReachFinalDestination(agent);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
            }
        }

        agent.walker.SetLastPosition();


    }

    public override void EndPerformAction(SAP_Scheduler_NPC agent)
    {
        agent.animator.SetBool(agent.isCrafting_hash, false);
        agent.offScreenPosMoved = true;
        agent.lastValidNode = currentNode;
        currentNode = null;
        path.Clear();
        destinationReached = false;
        particlesOn = false;
        setupParticles.Stop();
    }

    public override void ReachFinalDestination(SAP_Scheduler_NPC agent)
    {
        mazeCreator.ResetMaze();
        agent.offScreenPosMoved = true;
        agent.isDeviating = false;
        destinationReached = true;
        agent.animator.SetBool(agent.isCrafting_hash, true);
        agent.animator.SetFloat(agent.velocityX_hash, 0);
        agent.walker.currentDirection = Vector2.zero;
    }

}
