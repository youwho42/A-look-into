using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
	public class SAP_ANIMAL_Sleep : SAP_Action
	{

        SAP_WorldBeliefStates worldStates;
        public override void StartPerformAction(SAP_Scheduler_ANIMAL agent)
        {

            if (agent.removeFromMusicAtHome)
            {
                agent.musicGeneratorItem.RemoveFromDictionary();
                agent.musicGeneratorItem.isActive = false;
            }

            worldStates = SAP_WorldBeliefStates.instance;
            
            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, true);
            if (agent.flier != null)
                agent.flier.enabled = false;
            
            if (agent.sounds != null)
            {
                if (agent.sounds.continuous)
                    agent.sounds.mute = true;
            }
        }
        public override void PerformAction(SAP_Scheduler_ANIMAL agent)
        {
            if (worldStates.worldStates.TryGetValue("AnimalDay", out bool isDay))
            {
                bool d = !agent.isNocturnal && isDay || agent.isNocturnal && !isDay;
                agent.currentGoalComplete = d;
            }
            agent.animator.SetBool(agent.landed_hash, true);
            agent.animator.SetBool(agent.walking_hash, false);
            agent.animator.SetBool(agent.sleeping_hash, true);
        }
        public override void EndPerformAction(SAP_Scheduler_ANIMAL agent)
        {
            
            if (agent.currentDisplacementSpot != null)
                agent.currentDisplacementSpot.isInUse = false;
            agent.currentDisplacementSpot = null;

            if (agent.sounds != null)
            {
                if (agent.sounds.continuous)
                    agent.sounds.mute = false;
            }


            if (agent.removeFromMusicAtHome)
            {
                agent.musicGeneratorItem.isActive = true;
                agent.musicGeneratorItem.AddToDictionary();
            }


            if (agent.flier != null)
                agent.flier.enabled = true;
            agent.SetBeliefState("AtHome", false);
        }

       
    }

}