using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.StatSystem
{
    public class StatHandler : MonoBehaviour
    {

        public List<StatObject> statObjects = new List<StatObject>();

        private void Start()
        {
            GameEventManager.onTimeTickEvent.AddListener(TickModifierTimers);
            ResetStats();
        }
        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(TickModifierTimers);
        }

        void TickModifierTimers(int tick)
        {
            foreach (var stat in statObjects)
            {
                stat.DecreaseModifiersTimer();
                stat.ConstantDeclineTick();
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            foreach (var stat in statObjects)
            {
                if (stat != modifier.StatToModify)
                    continue;
                modifier.finalModifierAmount = modifier.ModifierAmount;
                stat.AddModifier(modifier);
                
                break;
            }
            GameEventManager.onStatUpdateEvent.Invoke();
        }

        public void AddModifiableModifier(StatModifier modifier)
        {
            foreach (var stat in statObjects)
            {
                if (stat != modifier.StatToModify)
                    continue;
                bool alreadyExists = false;
                List<StatModifier> mods = stat.GetModifierList();
                foreach (var mod in mods)
                {
                    if (mod != modifier)
                        continue;

                    alreadyExists = true;
                    mod.finalModifierAmount += modifier.ModifierAmount;

                    break;
                }
                if (!alreadyExists)
                {
                    modifier.finalModifierAmount = modifier.ModifierAmount;
                    stat.AddModifier(modifier);
                }
                stat.SetCurrent(stat.GetRawCurrent() + modifier.ModifierAmount);

                break;
            }
            GameEventManager.onStatUpdateEvent.Invoke();
        }

        public void RemoveModifiableModifier(StatModifier modifier)
        {
            foreach (var stat in statObjects)
            {
                if (stat != modifier.StatToModify)
                    continue;
                List<StatModifier> mods = stat.GetModifierList();
                foreach (var mod in mods)
                {
                    if (mod != modifier)
                        continue;

                    if(mod.finalModifierAmount <= modifier.ModifierAmount)
                        stat.RemoveModifier(mod);
                    else
                        mod.finalModifierAmount -= modifier.ModifierAmount;

                    break;
                }
                //stat.SetCurrent(stat.GetRawCurrent() - modifier.ModifierAmount);
                break;
            }
            GameEventManager.onStatUpdateEvent.Invoke();
        }



        public void ChangeStat(StatChanger changer)
        {
            foreach (var stat in statObjects)
            {
                if (stat != changer.StatToModify)
                    continue;

                if (changer.ModifierType == ModifierType.RawNumber)
                    stat.ChangeStatRaw(changer);
                else
                    stat.ChangeStatPercent(changer);
            }
            
        }

        void ResetStats()
        {
            foreach (var stat in statObjects)
            {
                stat.ResetStat();
                stat.RemoveAllModifiers();
            }
        }

        public float GetStatMaxModifiedValue(string statName)
        {
            foreach (var stat in statObjects)
            {
                if(stat.Name == statName)
                    return stat.GetModifiedMax();
            }
            return 1;
        }

        public float GetStatCurrentModifiedValue(string statName)
        {
            foreach (var stat in statObjects)
            {
                if (stat.Name == statName)
                    return stat.GetModifiedCurrent();
            }
            return 1;
        }

        
    }
}

