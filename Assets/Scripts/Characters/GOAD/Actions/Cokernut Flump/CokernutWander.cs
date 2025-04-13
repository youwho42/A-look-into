using UnityEngine;

namespace Klaxon.GOAD
{
	public class CokernutWander : GOAD_Action
	{
        public float wanderDistance = 1;
        Vector3 wanderDestination;
        float deviateTimer;
        bool trueDestination;
        public GOAD_ScriptableCondition fixableNearCondition;
        public GOAD_ScriptableCondition craftingStationNearCondition;
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
            
        }
        
        public override void PerformAction(GOAD_Scheduler_CF agent)
        {
            base.PerformAction(agent);

            if (agent.manager.SpawnTimerExpired())
            {
                agent.FleePlayer(PlayerInformation.instance.player);
                return;
            }
                

            if (agent.lastValidTile != agent.currentTilePosition.position)
            {
                agent.lastValidTile = agent.currentTilePosition.position;
                // check fixables for closest and in range
                agent.currentFixable = agent.GetClosestFixable();
                agent.currentCraftingStation = agent.GetClosestCraftingStation();
                if(agent.currentFixable != null && agent.currentCraftingStation != null)
                {
                    var da = (agent.currentFixable.transform.position - transform.position).sqrMagnitude;
                    var db = (agent.currentCraftingStation.transform.position - transform.position).sqrMagnitude;
                    if (da > db)
                        agent.currentFixable = null;
                    else
                        agent.currentCraftingStation = null;
                }
                if (agent.currentFixable != null)
                {
                    success = false;
                    agent.SetActionComplete(true);
                    agent.SetBeliefState(fixableNearCondition.Condition, fixableNearCondition.State);
                    return;
                }
                if (agent.currentCraftingStation != null)
                {
                    success = false;
                    agent.SetActionComplete(true);
                    agent.SetBeliefState(craftingStationNearCondition.Condition, craftingStationNearCondition.State);
                    return;
                }
            }
            



            agent.animator.SetBool(agent.isGrounded_hash, agent.walker.isGrounded);
            //agent.animator.SetBool(agent.isRunning_hash, agent.walker.isRunning);
            agent.animator.SetFloat(agent.velocityY_hash, agent.walker.isGrounded ? 0 : agent.walker.displacedPosition.y);
            agent.animator.SetFloat(agent.velocityX_hash, 1);

            if (agent.offScreen || agent.sleep.isSleeping)
            {
                
                agent.HandleOffScreenWander(this, wanderDestination);
                return;
            }

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
                var ooze = trueDestination ? Random.value > 0.5f : false;
                success = ooze;
                agent.SetActionComplete(true);
            }

            agent.walker.SetLastPosition();


            
        }

        public override void EndAction(GOAD_Scheduler_CF agent)
        {
            base.EndAction(agent);
            agent.animator.SetFloat(agent.velocityX_hash, 0);

            agent.walker.currentDirection = Vector2.zero;
        }


        public bool SetRandomDestinationFrom(GOAD_Scheduler_CF agent)
        {

            Vector2 rand = Random.insideUnitCircle * wanderDistance;
            var dir = agent.currentDestructableLocation - transform.position;
            var dist = dir.sqrMagnitude;
            Vector2 offset = dir.normalized * Mathf.Clamp(dist, 0.5f, 3.0f);

            offset = (Vector2)transform.position + offset;
            offset = offset + rand;
            var d = agent.walker.currentTilePosition.groundMap.WorldToCell(offset);
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