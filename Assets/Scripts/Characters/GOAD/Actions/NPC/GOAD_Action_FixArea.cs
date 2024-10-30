using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
	{
	public class GOAD_Action_FixArea : GOAD_Action
	{

        float positionTimer;
        public Vector2 positionTimerMinMax;


        public FixVillageDesk villageDesk;
        public GOAD_ScriptableCondition fixAreaCondition;
        FixVillageArea currentArea;
        
        bool fixing;
        bool areaFixed;
        Vector3 basePos;
        FixVillageArea GetFixableArea()
        {
            FixVillageArea fixArea = null;
            foreach (var area in villageDesk.fixableAreas)
            {
                if (area.isFixing)
                {
                    fixArea = area;
                    break;
                }

            }
            return fixArea;
        }

        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            GameEventManager.onTimeTickEvent.AddListener(AddTick);
            fixing = false;
            base.StartAction(agent);
            currentArea = GetFixableArea();

            positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
            basePos = transform.position;

            // Setting up the seat for when we leave
            if (currentArea != null)
            {
                var target = currentArea.reachNode;
                agent.currentNode = currentArea.fixingNode;
                agent.nodePath.Clear();
                agent.currentPathIndex = 0;
                agent.nodePath = agent.currentNode.FindPath(target);
            }
            agent.isBusy = true;
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            if (areaFixed)
            {
                GetUpAndLeave(agent);
                return;
            }

            if(!fixing)
            {
                currentArea.fixingSounds.StartSoundsNoTimer();
                currentArea.fixingEffect.Play();
                agent.animator.SetBool(agent.isCrafting_hash, true);
                agent.animator.SetFloat(agent.velocityX_hash, 0);
                agent.walker.currentDirection = Vector2.zero;
                fixing = true;
            }


            positionTimer -= Time.deltaTime;
            if (positionTimer < 0)
            {
                positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
                Vector2 randomPos = GetPointInsideCollider(currentArea.areaCollider);
                Vector3 finalPos = new Vector3(randomPos.x, randomPos.y, transform.position.z);
                transform.position = finalPos;

                float rand = Random.Range(0.0f, 1.0f);
                if (rand >= .6f)
                    agent.walker.Flip();
            }


            if (currentArea.fixTimer >= currentArea.ticksToFix)
            {
                

                currentArea.activeArea.SetActive(true);
                currentArea.inactiveArea.SetActive(false);
                currentArea.isActive = true;
                currentArea.isFixing = false;
                currentArea.undertakingObject.TryCompleteTask(currentArea.taskObject);
                foreach (var tilePos in currentArea.pathfindingTilePositions)
                {
                    PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[tilePos].walkable = true;
                }
                GOAD_WorldBeliefStates.instance.SetWorldState(currentArea.GOAD_CompletedCondition.Condition, currentArea.GOAD_CompletedCondition.State);
                GOAD_WorldBeliefStates.instance.SetWorldState(fixAreaCondition.Condition, fixAreaCondition.State);
                transform.position = basePos;
                areaFixed = true;
            }

            

        }

        void AddTick(int tick)
        {
            if (!areaFixed)
                currentArea.fixTimer++;
        }

        private void GetUpAndLeave(GOAD_Scheduler_NPC agent)
        {
            agent.animator.SetBool(agent.isCrafting_hash, false);
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

                agent.AStarDeviate(this);
                return;

            }


            if (agent.nodePath.Count > 0)
            {
                agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
            }
            agent.walker.SetDirection();

            if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.01f)
            {
                if (agent.currentPathIndex < agent.nodePath.Count - 1)
                {
                    agent.currentPathIndex++;
                    agent.currentNode = agent.nodePath[agent.currentPathIndex];
                    agent.walker.currentDestination = agent.nodePath[agent.currentPathIndex].transform.position;
                }
                else if (agent.currentPathIndex >= agent.nodePath.Count - 1)
                {
                    agent.lastValidNode = agent.currentNode;
                    success = true;
                    agent.SetActionComplete(true);

                }
            }

            agent.walker.SetLastPosition();

        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            GameEventManager.onTimeTickEvent.RemoveListener(AddTick);
            currentArea = null;
            fixing = false;
            areaFixed = false;
            agent.isBusy = false;
        }

        public Vector2 GetPointInsideCollider(Collider2D collider)
        {
            Vector2 point;
            point.x = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
            point.y = Random.Range(collider.bounds.min.y, collider.bounds.max.y);

            point = collider.ClosestPoint(point);

            return point;
        }
    } 
}
