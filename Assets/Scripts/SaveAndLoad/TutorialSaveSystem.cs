using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class TutorialSaveSystem : MonoBehaviour, ISaveable
    {

        public TutorialUI craftingTutorial;
        public TutorialUI researchTutorial;
        public TutorialUI sleepTutorial;

        public object CaptureState()
        {
            
            return new SaveData
            {
                craftingHasShownTutorial = craftingTutorial.hasShownTutorial,
                researchHasShownTutorial = researchTutorial.hasShownTutorial,
                sleepHasShownTutorial = sleepTutorial.hasShownTutorial
                
            };
            
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            craftingTutorial.SetFromSave(saveData.craftingHasShownTutorial);
            researchTutorial.SetFromSave(saveData.researchHasShownTutorial);
            sleepTutorial.SetFromSave(saveData.sleepHasShownTutorial);
        }



        [Serializable]
        private struct SaveData
        {
            public bool craftingHasShownTutorial;
            public bool researchHasShownTutorial;
            public bool sleepHasShownTutorial;
            
        }
    } 
}
