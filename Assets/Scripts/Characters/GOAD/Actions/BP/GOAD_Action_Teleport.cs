using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_Teleport : GOAD_Action
	{

        float timer;

        public LayerMask obstacleLayer;

        public override void StartAction(GOAD_Scheduler_BP agent)
        {
            base.StartAction(agent);
            agent.walker.currentDirection = Vector2.zero;
            agent.animator.SetBool(agent.walking_hash, false);
            agent.interactor.canInteract = false;

            agent.Disolve(false);
        }
        public override void PerformAction(GOAD_Scheduler_BP agent)
        {
            base.PerformAction(agent);
            if (timer < 1.5f)
                timer += Time.deltaTime;
            else
            {
                SetPositionNearPlayer(agent);
                agent.Disolve(true);
                success = true;
                agent.SetActionComplete(true);
            }
            agent.walker.ResetLastPosition();
        }
        
        public override void EndAction(GOAD_Scheduler_BP agent)
        {
            base.EndAction(agent);
            timer = 0;
            agent.interactor.canInteract = true;
        }

        void SetPositionNearPlayer(GOAD_Scheduler_BP agent)
        {
            Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            Vector3 potentialSpot = PlayerInformation.instance.player.position + (Vector3)offset;
            if (agent.walker.tileBlockInfo != null)
            {
                foreach (var tile in agent.walker.tileBlockInfo)
                {

                    if (tile.direction == Vector3Int.zero)
                    {
                        var hit = Physics2D.OverlapPoint(potentialSpot, obstacleLayer);
                        bool hitValid = hit != null;
                        if (hitValid)
                        {
                            if (hit.TryGetComponent(out DrawZasYDisplacement disp))
                                hitValid = disp.positionZ > 0;
                        }
                        

                        if (tile.isValid && !hitValid)
                        {
                            transform.position = potentialSpot;
                            agent.walker.currentLevel = (int)transform.position.z - 1;
                            agent.walker.currentTilePosition.position = agent.walker.currentTilePosition.GetCurrentTilePosition(transform.position);
                            //var p = transform.position;

                            //p.z = agent.walker.currentTilePosition.position.z + 1;

                            //transform.position = p;
                        }
                        else
                            SetPositionNearPlayer(agent);
                    }
                }
            }

        }

    } 
}
