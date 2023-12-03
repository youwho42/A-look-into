using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_BP_Teleport : SAP_Action
    {
        float timer;
        public override void StartPerformAction(SAP_Scheduler_BP agent)
        {
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.interactor.canInteract = false;

            agent.Disolve(false);

            
        }
        public override void PerformAction(SAP_Scheduler_BP agent)
        {
            if (timer < 1.5f)
                timer += Time.deltaTime;
            else
            {
                SetPositionNearPlayer(agent);
                agent.Disolve(true);
                agent.SetBeliefState("PlayerFar", false);
                agent.currentGoalComplete = true;
            }
            agent.walker.ResetLastPosition();
        }
        public override void EndPerformAction(SAP_Scheduler_BP agent)
        {
            timer = 0;
            agent.interactor.canInteract = true;

        }

        
        void SetPositionNearPlayer(SAP_Scheduler_BP agent)
        {
            Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            transform.position = PlayerInformation.instance.player.position + (Vector3)offset;
            if (agent.walker.tileBlockInfo != null)
            {
                foreach (var tile in agent.walker.tileBlockInfo)
                {

                    if (tile.direction == Vector3Int.zero)
                    {
                        var hit = Physics2D.OverlapPoint(transform.position, agent.obstacleLayer);
                        if (tile.isValid && hit == null)
                        {
                            agent.walker.currentTilePosition.position = agent.walker.currentTilePosition.GetCurrentTilePosition(transform.position);
                            var p = transform.position;
                            
                            p.z = agent.walker.currentTilePosition.position.z + 1;
                            
                            transform.position = p;
                        }
                        else
                            SetPositionNearPlayer(agent);
                    }
                }
            }

        }


    }

}
