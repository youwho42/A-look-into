using UnityEngine;

namespace Klaxon.GOAD
{
	public class CokernutWander : GOAD_Action
	{
        public float wanderDistance = 1;
        Vector3 wanderDestination;
        float deviateTimer;
        bool trueDestination;
        public override void StartAction(GOAD_Scheduler_CF agent)
        {
            base.StartAction(agent);
            trueDestination = true;
            deviateTimer = 0;
            if (!SetRandomDestinationFrom(agent))
            {
                success = false;
                agent.SetActionComplete(true);
            }
            //agent.walker.isRunning = true;
        }
        
        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);

            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            //agent.animator.SetBool(agent.isRunning_hash, agent.walker.isRunning);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.walker.isStuck || agent.isDeviating)
            {
                deviateTimer += Time.deltaTime;
                if (deviateTimer >= 3.0f)
                {
                    success = false;
                    agent.SetActionComplete(true);
                    return;
                }
                if (!agent.walker.jumpAhead)
                {
                    agent.Deviate();
                    trueDestination = false;
                    return;
                }

            }



            agent.walker.SetWorldDestination(wanderDestination);
            agent.walker.SetDirection();
            if (agent.walker.CheckDistanceToDestination() <= 0.02f)
            {
                
                //    agent.flumpOozeManager.StartOoze();
                success = trueDestination;
                agent.SetActionComplete(true);
            }

            agent.walker.SetLastPosition();


            
        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
        }


        public bool SetRandomDestinationFrom(GOAD_Scheduler_CF agent)
        {

            Vector2 rand = Random.insideUnitCircle * wanderDistance;
            var d = agent.walker.currentTilePosition.groundMap.WorldToCell(new Vector2(agent.transform.position.x + rand.x, agent.transform.position.y + rand.y));
            for (int z = agent.walker.currentTilePosition.groundMap.cellBounds.zMax; z > agent.walker.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {

                d.z = z;
                if (agent.walker.currentTilePosition.groundMap.GetTile(d) != null)
                {

                    var dif = agent.walker.currentTilePosition.position.z - z;
                    if (Mathf.Abs(dif) > 1)
                        return false;

                    wanderDestination = agent.walker.GetTileWorldPosition(d);
                    wanderDestination += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 1f);
                    var hits = Physics2D.OverlapCircleAll(wanderDestination, 0.05f, LayerMask.GetMask("Obstacle"), wanderDestination.z, wanderDestination.z);

                    return hits.Length <= 0;

                }
            }
            return false;
        }

    }

}