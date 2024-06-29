using QuantumTek.QuantumInventory;
using System;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class CraftingFuelSaveSystem : MonoBehaviour, ISaveable
	{
        public QI_CraftingHandler craftingHandler;

        
        public object CaptureState()
        {
            if(craftingHandler.currentFuel != null)
            {
                return new SaveData
                {
                    fuelName = craftingHandler.currentFuel.Name,
                    fuelAmount = craftingHandler.currentFuelAmount,
                    fuelTick = craftingHandler.currentFuelTick,
                    maxTick = craftingHandler.currentFuelTick
                };
            }
            else
            {
                return new SaveData
                {
                    fuelName = "",
                    fuelAmount = craftingHandler.currentFuelAmount,
                    fuelTick = craftingHandler.currentFuelTick,
                    maxTick = craftingHandler.currentFuelTick
                };
            }
            
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            if(saveData.fuelName != "")
                craftingHandler.currentFuel = AllItemsDatabaseManager.instance.allItemsDatabase.GetItem(saveData.fuelName);
            craftingHandler.currentFuelAmount = saveData.fuelAmount;
            craftingHandler.currentFuelTick = saveData.fuelTick;
            craftingHandler.currentMaxTick = saveData.maxTick;
        }

        [Serializable]
        private struct SaveData
        {
            public string fuelName;
            public int fuelAmount;
            public int fuelTick;
            public int maxTick;
        }
    }

}