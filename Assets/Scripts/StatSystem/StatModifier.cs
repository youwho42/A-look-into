using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.StatSystem
{
    [Serializable]
    public enum ModifierType
    {
        RawNumber,
        Percent
    }
    [Serializable]
    public enum ModifierDestination
    {
        MaxAmount,
        CurrentAmount
    }


    


    [CreateAssetMenu(menuName = "Klaxon/Stat Modifier", fileName = "New-Stat-Modifier")]
    [Serializable]	
	public class StatModifier : ScriptableObject
	{

        public StatObject StatToModify;
        public ModifierType ModifierType;
        public ModifierDestination ModifierDestination;
        public float ModifierAmount;
        public bool TimedModifier;
        [ConditionalHide("TimedModifier", true)]
        public int ModifierDuration;
        int ModifierTimer;

        
        public void IncreaseTimer(int timeAmount)
        {
            ModifierTimer += timeAmount;
        }

        public bool DecreaseModifierTimer()
        {
            if(ModifierTimer > 0)
            {
                ModifierTimer--;
                return true;
            }
                
            ModifierTimer = 0;
            return false;
        }
    }
}
