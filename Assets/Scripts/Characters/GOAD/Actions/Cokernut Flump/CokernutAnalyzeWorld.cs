using UnityEngine;

namespace Klaxon.GOAD
{
    public class CokernutAnalyzeWorld : GOAD_Action
    {
        bool hasSpawned;
        bool hasDisappeared;
        float travelTime = 0;
        bool isActive;
        Vector3 spawnPosition;
        public override void StartAction(GOAD_Scheduler_CF agent)
        {
            base.StartAction(agent);
            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.walker.currentDirection = Vector2.zero;
            hasSpawned = false;
            hasDisappeared = false;
            travelTime = 0;
            isActive = false;
            if (!agent.manager.CheckCanAppear())
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
            else
            {
                agent.manager.SetNextAppearance();
                if (!agent.manager.GetRandomCokernutInteractablePosition(out Vector3 pos))
                {
                    
                    success = false;
                    agent.SetActionComplete(true);
                    return;
                }
                agent.manager.SetSpawnTime();
                isActive = true;
                agent.currentDestructableLocation = pos;
                spawnPosition = agent.manager.GetRandomPositionAround(pos);
            }
        }

        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);
            if (!isActive)
            {
                success = false;
                agent.SetActionComplete(true);
                return;
            }
                
            
            if (!hasSpawned)
            {
                if (!hasDisappeared)
                {
                    hasDisappeared = true;
                    DissolveEffect.instance.StartDissolve(agent.walker.characterRenderer.material, 1.0f, false);
                }
                
                travelTime += Time.deltaTime;
                if (travelTime >= 1.0f)
                {
                    agent.transform.position = spawnPosition;
                    agent.currentTilePosition.position = agent.currentTilePosition.GetCurrentTilePosition(agent.transform.position);
                    agent.walker.currentLevel = (int)agent.transform.position.z - 1;
                    DissolveEffect.instance.StartDissolve(agent.walker.characterRenderer.material, 1.0f, true);
                    hasSpawned = true;
                }
                return;
            }
            success = true;
            agent.SetActionComplete(true);

        }

        
    }
}
