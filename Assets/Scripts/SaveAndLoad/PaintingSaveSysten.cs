using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class PaintingSaveSysten : MonoBehaviour, ISaveable
	{
        public RestorePainting restorePainting;

        public object CaptureState()
        {
            List<bool> a = new List<bool>();
            List<bool> c = new List<bool>();
            
            foreach (var item in restorePainting.ingredients)
            {
                a.Add(item.activated);
                c.Add(item.complete);
            }
            return new SaveData
            {
                layerActivated = a,
                layerComplete = c
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < restorePainting.ingredients.Count; i++)
            {
                restorePainting.ingredients[i].activated = saveData.layerActivated[i];
                restorePainting.ingredients[i].complete = saveData.layerComplete[i];
            }
            restorePainting.GetIsFinished();
        }

        [Serializable]
        private struct SaveData
        {
            public List<bool> layerActivated;
            public List<bool> layerComplete;
            

        }
    }

}