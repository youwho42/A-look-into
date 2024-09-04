using Klaxon.MazeTech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Action_SetMaze : GOAD_Action
	{
        public NavigationNode mazeEntranceNode;

        float positionTimer;
        public Vector2 positionTimerMinMax;
        public MazeCreator mazeCreator;
        public ParticleSystem setupParticles;
        bool particlesOn;
        CycleTicks mazeCycle;
        public int mazeFixTicks;
        RealTimeDayNightCycle dayNightCycle;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            dayNightCycle = RealTimeDayNightCycle.instance;
            agent.animator.SetBool(agent.isCrafting_hash, true);
            mazeCreator.ResetMaze();
            particlesOn = false;
            setupParticles.Stop();
            positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
            mazeCycle = dayNightCycle.GetCycleTime(mazeFixTicks);

        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            if(dayNightCycle.currentTimeRaw >= mazeCycle.tick && dayNightCycle.currentDayRaw == mazeCycle.day)
            {
                success = true;
                agent.SetActionComplete(true);
                return;
            }


            if (!particlesOn)
            {
                setupParticles.Play();
                particlesOn = true;
            }

            positionTimer -= Time.deltaTime;
            if (positionTimer < 0)
            {
                positionTimer = Random.Range(positionTimerMinMax.x, positionTimerMinMax.y);
                int r = Random.Range(0, mazeCreator.allTiles.Count);
                Vector3 randomTile = mazeCreator.allTiles[r].TileCenter;
                randomTile.z = transform.position.z;
                transform.position = randomTile;
            }
        }

        public override void SucceedAction(GOAD_Scheduler_NPC agent)
        {
            base.SucceedAction(agent);
        }

        public override void FailAction(GOAD_Scheduler_NPC agent)
        {
            base.FailAction(agent);
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            mazeCreator.StartMazeCreation();
            setupParticles.Stop();
            agent.animator.SetBool(agent.isCrafting_hash, false);
            transform.position = mazeEntranceNode.transform.position;
            
        }
    } 
}
