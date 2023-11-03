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
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            foreach (var stat in statObjects)
            {
                if (stat != modifier.StatToModify)
                    continue;
                
                stat.AddModifier(modifier);
                
                break;
            }
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
                stat.RemoveAllModifiers();
            }
        }
        
    }
}

