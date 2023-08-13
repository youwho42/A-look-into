using QuantumTek.QuantumInventory;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class PlantLifeSaveSystem : MonoBehaviour, ISaveable
    {
        public PlantLife plantLife;


        public object CaptureState()
        {


            return new SaveData
            {
                currentCycle = plantLife.currentCycle,
                nextCycleDay = plantLife.cycle.day,
                nextCycleTick = plantLife.cycle.tick
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            plantLife.currentCycle = saveData.currentCycle;
            plantLife.cycle = new CycleTicks();
            plantLife.cycle.day = saveData.nextCycleDay;
            plantLife.cycle.tick = saveData.nextCycleTick;
            Invoke("SetPlantLife", 0.5f);

        }

        void SetPlantLife()
        {
            plantLife.SetPlantArea();
            plantLife.SetSprites();
        }

        [Serializable]
        private struct SaveData
        {
            public int currentCycle;
            public int nextCycleDay;
            public int nextCycleTick;
        }
    } 
}