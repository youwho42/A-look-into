using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.StatSystem
{


    [CreateAssetMenu(menuName = "Klaxon/Stat Object", fileName = "New_Stat_Object")]
    public class StatObject : ScriptableObject
    {
        public string Name;
        [SerializeField]
        float MaxAmount;
        [SerializeField]
        float CurrentAmount;
        List<StatModifier> Modifiers = new List<StatModifier>();

        /// <summary>
        /// Increases the Stat Destination by the given RawNumber 
        /// </summary>
        /// <param name="changer"></param>
        public void ChangeStatRaw(StatChanger changer)
        {
            if(changer.ModifierDestination == ModifierDestination.CurrentAmount)
                CurrentAmount = Mathf.Clamp(CurrentAmount + changer.ModifierAmount, 0, MaxAmount);
            else
                MaxAmount += changer.ModifierAmount;
        }


        /// <summary>
        /// Multiplies the Destination Amount by the given amount
        /// </summary>
        /// <param name = "changer" ></ param >
        public void ChangeStatPercent(StatChanger changer)
        {
            if (changer.ModifierDestination == ModifierDestination.CurrentAmount)
                CurrentAmount = Mathf.Clamp(CurrentAmount * changer.ModifierAmount, 0, MaxAmount);
            else
                MaxAmount *= changer.ModifierAmount;
            
        }

        /// <summary>
        /// Returns the MaxAmount plus its modifiers
        /// </summary>
        /// <returns></returns>
        public float GetMax()
        {
            float amount = 0;
            float percent = 0;
            float final = MaxAmount;
            foreach (var mod in Modifiers)
            {
                if (mod.ModifierType == ModifierType.RawNumber)
                {
                    amount += mod.ModifierAmount;
                    final = MaxAmount + amount;
                }
                else
                {
                    percent += mod.ModifierAmount;
                    final = MaxAmount * (percent + 1);
                }

            }
            return final;
        }

        /// <summary>
        /// Returns the CurrentAmount
        /// </summary>
        /// <returns></returns>
        public float GetCurrent()
        {
            float amount = 0;
            float percent = 0;
            foreach (var mod in Modifiers)
            {
                if (mod.ModifierType == ModifierType.RawNumber)
                    amount += mod.ModifierAmount;
                else
                {
                    percent += mod.ModifierAmount;
                    amount += CurrentAmount * percent;
                }

            }
            return CurrentAmount + amount;
        }

        public void AddModifier(StatModifier modifier)
        {
            if (!Modifiers.Contains(modifier))
                Modifiers.Add(modifier);
            modifier.IncreaseTimer(modifier.ModifierDuration);
        }


        public void RemoveModifier(StatModifier modifier)
        {
            if (Modifiers.Contains(modifier))
                Modifiers.Remove(modifier);
        }

        public void RemoveAllModifiers()
        {
            Modifiers.Clear();
        }



        public void DecreaseModifiersTimer()
        {
            List<StatModifier> modsToRemove = new List<StatModifier>();
            foreach (var mod in Modifiers)
            {
                if (!mod.DecreaseModifierTimer())
                    modsToRemove.Add(mod);
            }
            foreach (var mod in modsToRemove)
            {
                RemoveModifier(mod);
            }
            
        }
    }
}
