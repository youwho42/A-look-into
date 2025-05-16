using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPIndicateAStar : GOAD_Action
    {

        Vector3 markerPosition;
        Vector2 indicatorPos;
        PlayerInformation player;
        PlayerMarkerTextureMap playerMarkerTextureMap;
        
        float timer;
        Vector2 minMaxThoughtBubbleTime = new Vector2(30, 60);
        float thoughtBubbleTime;
        GridManager grid;


        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            timer = 0;
            agent.interactor.canInteract = false;
            agent.animator.SetBool(agent.walking_hash, true);
            thoughtBubbleTime = Random.Range(minMaxThoughtBubbleTime.x, minMaxThoughtBubbleTime.y);

            player = PlayerInformation.instance;
            grid = GridManager.instance;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            SetMarkerDestination(agent);

            agent.currentPathIndex = 0;
            agent.aStarPath.Clear();

            
            if (agent.StartPositionValid())
                agent.SetAStarDestination(markerPosition, this);


            ContextSpeechBubbleManager.instance.SetContextBubble(4, agent.speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay"), false);


        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (agent.gettingPath)
                return;

            if (agent.currentPathIndex >= agent.aStarPath.Count)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }

            if (agent.walker.isStuck || agent.isDeviating)
            {

                agent.AStarDeviate(this);
                return;

            }


            if (agent.CheckNearPlayer(1.2f))
            {
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
                        success = true;
                        agent.SetActionComplete(true);
                    }
                }

                agent.walker.SetLastPosition();
            }
            else
            {
                agent.walker.currentDirection = Vector3.zero;
            }
            


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
