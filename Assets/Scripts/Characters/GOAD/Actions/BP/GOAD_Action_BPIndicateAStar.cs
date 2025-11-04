using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPIndicateAStar : GOAD_Action
    {

        Vector3 markerPosition;
        PlayerInformation player;
        PlayerMarkerTextureMap playerMarkerTextureMap;
        GridManager grid;
        float timer;
        Vector2 minMaxThoughtBubbleTime = new Vector2(30, 60);
        float thoughtBubbleTime;
        
        bool noPathTextShown;
        bool thisWayTextShown;
        bool destinationReached;
        

        bool needsNewStartPosition;
        bool hasNewOffset;
        Vector2 offsetCenter;
        Vector2 offset;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            timer = 0;
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, false);
            thoughtBubbleTime = Random.Range(minMaxThoughtBubbleTime.x, minMaxThoughtBubbleTime.y);
            noPathTextShown = false;
            thisWayTextShown = false;
            destinationReached = false;
            needsNewStartPosition = false;
            player = PlayerInformation.instance;
            grid = GridManager.instance;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            SetMarkerDestination(agent);

            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            agent.currentFinalDestination = markerPosition;
            if (agent.StartPositionValid())
                agent.SetAStarDestination(markerPosition, this, transform.position);
            else
            {
                needsNewStartPosition = true;
                agent.SetValidStartPosition(markerPosition, this);
            }
            
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (agent.gettingPath)
                return;

            if (needsNewStartPosition)
            {
                agent.aStarPath.Insert(0, agent.validStartPosition);
                needsNewStartPosition = false;
            }

            if (!agent.CheckNearPlayer(3))
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }


            if (destinationReached)
            {
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
                playerMarkerTextureMap.RemoveMarkerAtIndex(agent.indicatorIndex);
                ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorLocationFound"), false);
                timer += Time.deltaTime;
                if (timer > 2.5f)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                return;
            }

            if (agent.aStarPath.Count == 0)
            {
                timer += Time.deltaTime;
                if (!noPathTextShown)
                {
                    ContextSpeechBubbleManager.instance.SetContextBubble(3, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorNoPathFound"), false);
                    noPathTextShown = true;
                }
                if (timer >= 4)
                {
                    success = true;
                    agent.SetActionComplete(true);
                }
                
                return;
            }
            timer = 0;
            if (agent.currentPathIndex >= agent.aStarPath.Count)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }



            Vector2 dirFromPlayer = transform.position - player.player.position;
            var dir = Vector2.Dot(dirFromPlayer, player.playerController.currentDirection);


            if (agent.CheckNearPlayer(1.5f) || dir > 0.5f)
            {
                if (!thisWayTextShown)
                {
                    ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay"), false);
                    thisWayTextShown = true;
                }

                if (agent.walker.isStuck || agent.isDeviating)
                {

                    if (!agent.walker.jumpAhead)
                    {
                        agent.Deviate();
                        return;
                    }
                }


                if (!hasNewOffset)
                    GetNewOffset(agent);

                agent.walker.hasDeviatePosition = false;

                if (agent.aStarPath.Count > 0)
                    agent.walker.currentDestination = (agent.aStarPath[agent.currentPathIndex] + (Vector3)offsetCenter) + (Vector3)offset;


                agent.walker.walkSpeed = player.playerController.finalSpeed + (player.playerInput.isRunning ? 0.04f : 0.08f);

                agent.animator.SetBool(agent.walking_hash, true);
                agent.walker.SetDirection();

                if (agent.walker.CheckDistanceToDestination() <= 0.0036f)
                {
                    hasNewOffset = false;
                    agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];
                    if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                    {
                        agent.currentPathIndex++;
                        agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
                    }
                    else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                    {
                        timer = 0;
                        destinationReached= true;
                        return;
                    }
                }

                
            }
            else
            {
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
                if (agent.CheckNearPlayer(2f))
                {
                    success = false;
                    agent.SetActionComplete(true);
                }
                agent.walker.ResetLastPosition();
            }

            agent.walker.SetLastPosition();
        }

        //public override void EndAction(GOAD_Scheduler_BP agent)
        //{
        //    base.EndAction(agent);
        //}

        //public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_BP agent)
        //{
        //    base.AStarDestinationIsCurrentPosition(agent);
        //}

        private void GetNewOffset(GOAD_Scheduler_BP agent)
        {
            hasNewOffset = true;
            offsetCenter = Vector2.zero;
            offset = Vector2.zero;
            if (agent.currentPathIndex < agent.aStarPath.Count - 1)
            {
                offsetCenter = (agent.aStarPath[agent.currentPathIndex + 1] - agent.aStarPath[agent.currentPathIndex]).normalized;
                offsetCenter *= 0.15f;
                offset = Random.insideUnitCircle * 0.2f;
            }
        }

        public void SetMarkerDestination(GOAD_Scheduler_BP agent)
        {

            var marker = playerMarkerTextureMap.markers[agent.indicatorIndex];
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


        

    }
}
