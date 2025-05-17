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
            player = PlayerInformation.instance;
            grid = GridManager.instance;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            SetMarkerDestination(agent);

            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();
            agent.currentFinalDestination = markerPosition;
            if (agent.StartPositionValid())
                agent.SetAStarDestination(markerPosition, this);

        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (agent.gettingPath)
                return;

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
                ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, "Here we are!" /*LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay")*/, false);
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
                    ContextSpeechBubbleManager.instance.SetContextBubble(3, agent.speechBubbleTransform, "I couldn't find a path, you're on your own!"/*LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay")*/, false);
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

            




            if (agent.CheckNearPlayer(1f))
            {
                if (!thisWayTextShown)
                {
                    ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay"), false);
                    thisWayTextShown = true;
                }

                if (agent.aStarPath.Count > 0)
                    agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];

                agent.walker.walkSpeed = player.playerController.finalSpeed - 0.05f;

                agent.animator.SetBool(agent.walking_hash, true);
                agent.walker.SetDirection();

                if (agent.walker.CheckDistanceToDestination() <= agent.walker.checkTileDistance + 0.02f)
                {
                    agent.lastValidTileLocation = agent.aStarPath[agent.currentPathIndex];
                    if (agent.currentPathIndex < agent.aStarPath.Count - 1)
                    {
                        agent.currentPathIndex++;
                        agent.walker.currentDestination = agent.aStarPath[agent.currentPathIndex];
                    }
                    else if (agent.currentPathIndex >= agent.aStarPath.Count - 1)
                    {
                        destinationReached= true;
                        return;
                    }
                }

                
            }
            else
            {
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
                if (agent.CheckNearPlayer(1.3f))
                {
                    success = false;
                    agent.SetActionComplete(true);
                }
                agent.walker.ResetLastPosition();
            }

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
        }

        public override void AStarDestinationIsCurrentPosition(GOAD_Scheduler_BP agent)
        {
            base.AStarDestinationIsCurrentPosition(agent);
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
