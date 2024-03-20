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

        public object CaptureState()
        {
            
            return new SaveData
            {
                craftingHasShownTutorial = craftingTutorial.hasShownTutorial,
                researchHasShownTutorial = researchTutorial.hasShownTutorial,
                
            };
            
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            craftingTutorial.SetFromSave(saveData.craftingHasShownTutorial);
            researchTutorial.SetFromSave(saveData.researchHasShownTutorial);
        }



        [Serializable]
        private struct SaveData
        {
            public bool craftingHasShownTutorial;
            public bool researchHasShownTutorial;
            
        }
    } 
}
