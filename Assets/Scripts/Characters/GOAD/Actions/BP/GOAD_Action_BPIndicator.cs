using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPIndicator : GOAD_Action
    {
        Vector3 markerPosition;
        Vector2 indicatorPos;
        PlayerInformation player;
        PlayerMarkerTextureMap playerMarkerTextureMap;
        bool waiting;
        float leadDistanceOffset;
        bool distanceSet;
        float timer;
        Vector2 minMaxThoughtBubbleTime = new Vector2(30,60);
        float thoughtBubbleTime;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            timer = 0;
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            thoughtBubbleTime = Random.Range(minMaxThoughtBubbleTime.x, minMaxThoughtBubbleTime.y);
            SetMarkerDestination(agent);
            player = PlayerInformation.instance;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            leadDistanceOffset = Random.Range(-0.05f, 0.3f);
            distanceSet = true;
            ContextSpeechBubbleManager.instance.SetContextBubble(4, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay"), false);

        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (!MarkerActive(agent))
            {
                timer += Time.deltaTime;
                if (timer > 2.5f)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                
                return;
            }
            if (!agent.CheckNearPlayer(3))
            {
                success = false;
                agent.SetActionComplete(true);
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

            thoughtBubbleTime-=Time.deltaTime;
            if (thoughtBubbleTime <= 0)
            {
                thoughtBubbleTime = Random.Range(minMaxThoughtBubbleTime.x, minMaxThoughtBubbleTime.y);
                ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay"), false);

            }


            agent.walker.SetWorldDestination(agent.walker.currentDestination);

            if (agent.walker.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck && PlayerInformation.instance.playerInput.movement == Vector2.zero)
                waiting = true;

            if (waiting)
            {
                agent.walker.currentDirection = Vector2.zero;
                agent.animator.SetBool(agent.walking_hash, false);
                if (agent.walker.CheckDistanceToDestination() >= 0.04f + leadDistanceOffset*leadDistanceOffset)
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

            agent.walker.walkSpeed = player.playerController.finalSpeed - 0.05f;

            agent.animator.SetBool(agent.walking_hash, true);
            agent.walker.hasDeviatePosition = false;
            
            agent.walker.SetDirection();

            if (DistanceToMarker() <= 0.25f)
            {
                ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorLocationFound"), false);

                agent.walker.currentDirection = Vector2.zero;
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.jumpAhead = true;
                PlayerMarkerTextureMap.instance.RemoveMarkerAtIndex(agent.indicatorIndex);
            }

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            agent.justIndicated = false;
            agent.interactor.canInteract = true;

            agent.animator.SetBool(agent.walking_hash, false);
            agent.walker.isStuck = false;
            agent.isDeviating = false;

        }

        public void SetMarkerDestination(GOAD_Scheduler_BP agent)
        {

            GridManager grid = GridManager.instance;
            var marker = PlayerMarkerTextureMap.instance.markers[agent.indicatorIndex];
            var map = grid.groundMap.cellBounds;
            for (int i = map.zMax; i > map.zMin; i--)
            {
                Vector3Int pos = new Vector3Int(marker.terrainPosition.x, marker.terrainPosition.y, i);
                var t = grid.groundMap.GetTile(pos);
                if (t != null)
                {
                    pos.z = i + 1;
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

        bool MarkerActive(GOAD_Scheduler_BP agent)
        {
            return playerMarkerTextureMap.markers[agent.indicatorIndex].active;
        }

        float DistanceToMarker()
        {
            return NumberFunctions.GetDistanceV3(transform.position, markerPosition);
        }


    }
}
