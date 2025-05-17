using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_BPIndicatorFollowPlayer : GOAD_Action
    {
        Vector3 markerPosition;
        Vector2 indicatorPos;
        PlayerInformation player;
        PlayerMarkerTextureMap playerMarkerTextureMap;
        GridManager grid;
        Vector2 offset;
        Vector3 lastDestination;
        bool wrongWayTextShown;
        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            player = PlayerInformation.instance;
            grid = GridManager.instance;
            wrongWayTextShown = false;
            playerMarkerTextureMap = PlayerMarkerTextureMap.instance;
            SetMarkerDestination(agent);
        }

        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (!agent.CheckNearPlayer(3))
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            if (agent.CheckNearPlayer(.7f))
            {
                success= true;
                agent.SetActionComplete(true);
                return;
            }
            SetIndicatorPosition();

            if (agent.walker.isStuck || agent.isDeviating)
            {
                if (lastDestination == agent.walker.currentDestination)
                {
                    offset = indicatorPos;
                }
                lastDestination = agent.walker.currentDestination;

                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }
            }

            agent.walker.hasDeviatePosition = false;

            

            Vector2 dirFromPlayer = transform.position - player.player.position;
            var dir = Vector2.Dot(dirFromPlayer, player.playerController.currentDirection);
            if (dir < .5)
            {
                if (!wrongWayTextShown)
                {
                    ContextSpeechBubbleManager.instance.SetContextBubble(2, agent.speechBubbleTransform, "That's not the right way!"/*LocalizationSettings.StringDatabase.GetLocalizedString($"BP Speech", "IndicatorThisWay")*/, false);
                    wrongWayTextShown = true;
                }
                agent.walker.currentDestination = player.player.position + (Vector3)indicatorPos;
                
                agent.animator.SetBool(agent.walking_hash, true);
                agent.walker.SetWorldDestination(agent.walker.currentDestination);
                agent.walker.SetDirection();
                
            }
            else
            {
                
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
            }
            if(player.playerInput.movement == Vector2.zero)
            {
                agent.animator.SetBool(agent.walking_hash, false);
                agent.walker.currentDirection = Vector2.zero;
            }
            agent.walker.SetLastPosition();

            


        }

        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
        }


        public void SetIndicatorPosition()
        {
            var dir = markerPosition - player.player.position;
            dir = dir.normalized;
            indicatorPos = dir * 1.2f;

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
