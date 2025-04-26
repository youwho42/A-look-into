using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class SlickSculptureSaveSystem : MonoBehaviour, ISaveable
	{
        public SlickSculpturesManager sculptureManager;

        public object CaptureState()
        {
            List<bool> a = new List<bool>();
            List<bool> c = new List<bool>();
            List<int> t = new List<int>();
            List<int> q = new List<int>();
            for (int i = 0; i < sculptureManager.allSculptures.Count; i++)
            {
                for (int j = 0; j < sculptureManager.allSculptures[i].ingredients.Count; j++)
                {
                    a.Add(sculptureManager.allSculptures[i].ingredients[j].activated);
                    c.Add(sculptureManager.allSculptures[i].ingredients[j].complete);
                }
                t.Add(sculptureManager.allSculptures[i].ticks);
            }
            foreach (var item in sculptureManager.sculptureQueue)
            {
                q.Add(sculptureManager.allSculptures.IndexOf(item));
            }


            return new SaveData
            {
                layerActivated = a,
                layerComplete = c,
                paintingTicks = t,
                queueIndexes = q
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            int count = 0;
            for (int i = 0; i < sculptureManager.allSculptures.Count; i++)
            {
                for (int j = 0; j < sculptureManager.allSculptures[i].ingredients.Count; j++)
                {

                    sculptureManager.allSculptures[i].ingredients[j].activated = saveData.layerActivated[count];
                    sculptureManager.allSculptures[i].ingredients[j].complete = saveData.layerComplete[count];
                    count++;
                }

                sculptureManager.allSculptures[i].ticks = saveData.paintingTicks[i];
                sculptureManager.allSculptures[i].GetIsFinished();
            }
            foreach (var item in saveData.queueIndexes)
            {
                sculptureManager.sculptureQueue.Add(sculptureManager.allSculptures[item]);
            }

        }

        [Serializable]
        private struct SaveData
        {
            public List<bool> layerActivated;
            public List<bool> layerComplete;
            public List<int> paintingTicks;
            public List<int> queueIndexes;

        }
    }

}