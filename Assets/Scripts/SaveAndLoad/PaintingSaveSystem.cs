using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class PaintingSaveSystem : MonoBehaviour, ISaveable
	{
        public MuseumManager museumManager;

        public object CaptureState()
        {
            List<bool> a = new List<bool>();
            List<bool> c = new List<bool>();
            List<int> t = new List<int>();
            List<int> q = new List<int>();
            for (int i = 0; i < museumManager.allArtPieces.Count; i++)
            {
                for (int j = 0; j < museumManager.allArtPieces[i].ingredients.Count; j++)
                {
                    a.Add(museumManager.allArtPieces[i].ingredients[j].activated);
                    c.Add(museumManager.allArtPieces[i].ingredients[j].complete);
                }
                t.Add(museumManager.allArtPieces[i].ticks);
            }
            foreach (var item in museumManager.restorePaintingQueue)
            {
                q.Add(museumManager.allArtPieces.IndexOf(item));
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
            for (int i = 0; i < museumManager.allArtPieces.Count; i++)
            {
                for (int j = 0; j < museumManager.allArtPieces[i].ingredients.Count; j++)
                {
                    
                    museumManager.allArtPieces[i].ingredients[j].activated = saveData.layerActivated[count];
                    museumManager.allArtPieces[i].ingredients[j].complete = saveData.layerComplete[count];
                    count++;
                }
                
                museumManager.allArtPieces[i].ticks = saveData.paintingTicks[i];
                museumManager.allArtPieces[i].GetIsFinished();
            }
            foreach (var item in saveData.queueIndexes)
            {
                museumManager.restorePaintingQueue.Add(museumManager.allArtPieces[item]);
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