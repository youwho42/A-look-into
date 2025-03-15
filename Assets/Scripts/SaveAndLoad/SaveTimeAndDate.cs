using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SaveTimeAndDate : MonoBehaviour, ISaveable
    {
        
        public object CaptureState()
        {
            return new SaveData
            {
                currentTimeRaw = RealTimeDayNightCycle.instance.currentTimeRaw,
                currentDayRaw = RealTimeDayNightCycle.instance.currentDayRaw
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            RealTimeDayNightCycle.instance.SetDayTime(saveData.currentTimeRaw, saveData.currentDayRaw);
            CalendarManager.instance.SetDootOfWeek(saveData.currentDayRaw);
        }

        [Serializable]
        private struct SaveData
        {
            public int currentTimeRaw;
            public int currentDayRaw;
        }
    }

}