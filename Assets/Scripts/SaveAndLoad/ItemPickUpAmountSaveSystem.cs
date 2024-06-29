using Klaxon.Interactable;
using System;

using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class ItemPickUpAmountSaveSystem : MonoBehaviour, ISaveable
    {
        public InteractablePickUp pickUp;

        
        public object CaptureState()
        {
            return new SaveData
            {
                amount = pickUp.pickupQuantity
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            pickUp.pickupQuantity = saveData.amount;

        }

        [Serializable]
        private struct SaveData
        {
            public int amount;

        }
    } 
}
