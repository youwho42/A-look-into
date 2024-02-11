using Klaxon.StatSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class StatsSaveSystem : MonoBehaviour, ISaveable
    {
        public StatHandler statHandler;
        public StatModifierDatabase modifierDatabase;
        public object CaptureState()
        {
            
            List<float> tempMaxValue = new List<float>();
            List<float> tempCurrentValue = new List<float>();
            List<int> tempModQuantity = new List<int>();
            List<string> tempModNames = new List<string>();
            List<int> tempModsTimers = new List<int>();

            for (int i = 0; i < statHandler.statObjects.Count; i++)
            {
                
                tempMaxValue.Add(statHandler.statObjects[i].GetRawMax());
                tempCurrentValue.Add(statHandler.statObjects[i].GetRawCurrent());
                tempModQuantity.Add(statHandler.statObjects[i].GetModifiersCount());
                tempModNames.AddRange(statHandler.statObjects[i].GetModifierNames());
                tempModsTimers.AddRange(statHandler.statObjects[i].GetModifierTimers());
            }
            return new SaveData
            {
                
                maxValue = tempMaxValue,
                currentValue = tempCurrentValue,
                modQuantity = tempModQuantity,
                modsNames  = tempModNames,
                modsTimers = tempModsTimers
                
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            int index = 0;
            for (int i = 0; i < statHandler.statObjects.Count; i++)
            {
                statHandler.statObjects[i].SetMax(saveData.maxValue[i]);
                statHandler.statObjects[i].SetCurrent(saveData.currentValue[i]);
                for (int j = 0; j < saveData.modQuantity[i]; j++)
                {
                    statHandler.statObjects[i].AddModifierFromSave(modifierDatabase.GetModByName(saveData.modsNames[index]), saveData.modsTimers[index]);
                    index++;
                }
            }
            GameEventManager.onStatUpdateEvent.Invoke();
        }

        

        [Serializable]
        private struct SaveData
        {
            
            public List<float> maxValue;
            public List<float> currentValue;

            // mods
            public List<int> modQuantity;
            public List<string> modsNames;
            public List<int> modsTimers;
        }
    } 
}
