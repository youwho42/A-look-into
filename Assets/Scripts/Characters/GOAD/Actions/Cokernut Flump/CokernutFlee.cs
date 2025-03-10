using UnityEngine;

namespace Klaxon.GOAD
{
    public class CokernutFlee : GOAD_Action
    {
        Vector3 fleeDestination;
        float fleeTimer;
        bool disolving;
        public override void StartAction(GOAD_Scheduler_CF agent)
        {
            base.StartAction(agent);
            disolving = false;
            fleeTimer = 0;
            if (!SetFleeDestination(agent))
            {
                fleeTimer = 4f;
            }
            agent.walker.Bounce(4);
            agent.walker.isRunning = true;
            agent.isFleeing = true;
        }

        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);
            if (agent.sleep.isSleeping)
            {
                success = true;
                agent.SetActionComplete(true);
                agent.manager.nextAppearanceSet = false;
                gameObject.SetActive(false);
                return;
            }

            fleeTimer += Time.deltaTime;
            if(fleeTimer > 3.5f)
            {
                
                if (!disolving)
                {
                    DissolveEffect.instance.StartDissolve(agent.walker.characterRenderer.material, 1.0f, false);
                    disolving = true;
                }
                if(agent.walker.characterRenderer.material.GetFloat("_Fade") <= 0)
                {
                    success = true;
                    agent.SetActionComplete(true);
                    agent.manager.nextAppearanceSet = false;
                    gameObject.SetActive(false);
                }
                return;
            }

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            agent.animator.SetBool(agent.isRunning_hash, agent.walker.isRunning);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.walker.isStuck || agent.isDeviating)
            {
                
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    return;
                }

            }



            agent.walker.SetWorldDestination(fleeDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                SetFleeDestination(agent);
            }

            agent.walker.SetLastPosition();
        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
            agent.walker.isRunning = false;
            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.animator.SetBool(agent.isRunning_hash, false);
            agent.walker.currentDirection = Vector2.zero;
            agent.isFleeing = false;
        }


        public bool SetFleeDestination(GOAD_Scheduler_CF agent)
        {
            agent.fleeTransform = agent.fleeTransform == null ? PlayerInformation.instance.player : agent.fleeTransform;
            var dir = transform.position - agent.fleeTransform.position;
            Vector2 offset = dir.normalized * Random.Range(3, 4);
            float deg = Random.Range(-10, 10);
            float rad = Mathf.Deg2Rad * deg;
            Vector2 variation = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            offset += variation;
            
            offset = (Vector2)transform.position + offset;

            
            var d = agent.walker.currentTilePosition.groundMap.WorldToCell(offset);
            for (int z = agent.walker.currentTilePosition.groundMap.cellBounds.zMax; z > agent.walker.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {

                d.z = z;
                if (agent.walker.currentTilePosition.groundMap.GetTile(d) != null)
                {

                    var dif = agent.walker.currentTilePosition.position.z - z;
                    if (Mathf.Abs(dif) > 1)
                        return false;

                    fleeDestination = agent.walker.GetTileWorldPosition(d);
                    fleeDestination += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 1f);
                    var hits = Physics2D.OverlapCircleAll(fleeDestination, 0.05f, LayerMask.GetMask("Obstacle"), fleeDestination.z, fleeDestination.z);

                    return hits.Length <= 0;

                }
            }
            return false;
        }


    }
}
