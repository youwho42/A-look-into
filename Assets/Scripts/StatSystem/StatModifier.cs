using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

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
        CurrentAmount,
        ChangerAmount
    }


    


    [CreateAssetMenu(menuName = "Klaxon/Stat Modifier", fileName = "New-Stat-Modifier")]
    [Serializable]	
	public class StatModifier : ScriptableObject
	{
        public string ModifierName;
        public StatObject StatToModify;
        public ModifierType ModifierType;
        public ModifierDestination ModifierDestination;
        public float ModifierAmount;
        
        public float finalModifierAmount;
        public bool TimedModifier;
        [ConditionalHide("TimedModifier", true)]
        public int ModifierDuration;
        int MaxTimerAmount;
        int ModifierTimer;
        public Sprite modIcon;
        public Sprite modIconGrey;
        public LocalizedString EffectDescription;
        public void SetTimer(int amount)
        {
            ModifierTimer = amount;
        }
        public int GetTimer()
        {
            return ModifierTimer;
        }
        public int GetMaxTimer()
        {
            return MaxTimerAmount;
        }

        public void IncreaseTimer(int timeAmount)
        {
            ModifierTimer += timeAmount;
            MaxTimerAmount += timeAmount;
        }

        public bool DecreaseModifierTimer()
        {
            if(ModifierTimer > 0)
            {
                ModifierTimer--;
                GameEventManager.onStatUpdateEvent.Invoke();
                return true;
            }
                
            ModifierTimer = 0;
            GameEventManager.onStatUpdateEvent.Invoke();
            return false;
            
        }

        public void ResetModifier()
        {
            finalModifierAmount = ModifierAmount;
            ModifierTimer = 0;
            MaxTimerAmount = 0;
        }
    }
}
