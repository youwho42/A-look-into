using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_Indicate : SAP_Action
    {

        Vector3 markerPosition;
        Vector2 indicatorPos;
        PlayerInformation player;
        PlayerMarkerTextureMap playerMarkerTextureMap;
        bool waiting;
        float leadDistanceOffset;
        bool distanceSet;

        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);

            SetMarkerDestination(agent);
            player = PlayerInformation.instance;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            leadDistanceOffset = Random.Range(-0.05f, 0.3f);
            distanceSet = true;
        }

        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (!MarkerActive(agent))
            {
                agent.SetBeliefState("QuestOver", true);
                agent.currentGoalComplete = true;
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

            SetIndicatorPosition();
            agent.walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)indicatorPos;
            agent.walker.SetWorldDestination(agent.walker.currentDestination);

            if (agent.walker.CheckDistanceToDestination() <= 0.02f && PlayerInformation.instance.playerInput.movement == Vector2.zero)
                waiting = true;

            if (waiting)
            {
                agent.walker.currentDir = Vector2.zero;
                agent.animator.SetBool(agent.walking_hash, false);
                if (agent.walker.CheckDistanceToDestination() >= 0.2f + leadDistanceOffset)
                {
                    distanceSet = false;
                    waiting = false;
                }
                    
                return;
            }
            if (!distanceSet)
            {
                leadDistanceOffset = Random.Range(-0.05f, 0.3f);
                distanceSet = true;
            }
                
            agent.walker.walkSpeed = player.playerController.finalSpeed - 0.035f;

            agent.animator.SetBool(agent.walking_hash, true);
            agent.walker.hasDeviatePosition = false;
            if (agent.CheckPlayerDistance() > 3)
            {
                agent.SetBeliefState("PlayerFar", true);
                agent.currentGoalComplete = true;
                return;
            }

            agent.walker.SetDirection();

            if (DistanceToMarker() <= 0.5f)
            {
                PlayerMarkerTextureMap.instance.RemoveMarkerAtIndex(agent.indicatorIndex);
                agent.SetBeliefState("QuestOver", true);
                agent.currentGoalComplete = true;
            }

            agent.walker.SetLastPosition();
        }

        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            agent.justIndicated = false;
            agent.interactor.canInteract = true;

            agent.animator.SetBool(agent.walking_hash, false);
        }

        public void SetMarkerDestination(SAP_Scheduler_BP agent)
        {
            
            GridManager grid = GridManager.instance;
            var marker = PlayerMarkerTextureMap.instance.markers[agent.indicatorIndex];
            var map = grid.groundMap.cellBounds;
            for (int i = map.zMax; i >map.zMin; i--)
            {
                Vector3Int pos = new Vector3Int(marker.terrainPosition.x, marker.terrainPosition.y, i);
                var t = grid.groundMap.GetTile(pos);
                if (t != null)
                {
                    markerPosition = grid.groundMap.GetCellCenterWorld(pos);
                    
                    return;
                }
            }
        }

        public void SetIndicatorPosition()
        {
            var dir = markerPosition - PlayerInformation.instance.player.position;
            dir = dir.normalized;
            indicatorPos = dir * 1f;

        }

        bool MarkerActive(SAP_Scheduler_BP agent)
        {
            return playerMarkerTextureMap.markers[agent.indicatorIndex].active;
        }

        float DistanceToMarker()
        {
            return Vector2.Distance(transform.position, markerPosition);
        }
    } 
}