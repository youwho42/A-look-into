using System;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class CokernutFlumpManagerSaveSystem : MonoBehaviour, ISaveable
	{
        public CokernutManager cokernutManager;

        public object CaptureState()
        {

            CycleTicks newCycle = new CycleTicks();
            newCycle.tick = -1;
            newCycle.day = -1;
            if(cokernutManager.nextAppearance != null)
            {
                newCycle.tick = cokernutManager.nextAppearance.tick;
                newCycle.day = cokernutManager.nextAppearance.day;
            }
            return new SaveData
            {
                tick = newCycle.tick,
                day = newCycle.day,
                isSet = cokernutManager.nextAppearanceSet,
                isActive = cokernutManager.isActive
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            CycleTicks newCycle = new CycleTicks();
            newCycle.tick = saveData.tick;
            newCycle.day = saveData.day;
            if(newCycle.tick != -1)
                cokernutManager.nextAppearance = newCycle;
            cokernutManager.nextAppearanceSet = saveData.isSet;
            cokernutManager.ActivateCokernut(saveData.isActive);
        }

        [Serializable]
        private struct SaveData
        {
            public int tick;
            public int day;
            public bool isSet;
            public bool isActive;
        }
    } 
}
