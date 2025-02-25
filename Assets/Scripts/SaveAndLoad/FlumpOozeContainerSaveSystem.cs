using SerializableTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class FlumpOozeContainerSaveSystem : MonoBehaviour, ISaveable
    {
        public FlumpOozeContainer oozeContainer;

        public object CaptureState()
        {
            List<SVector3> pos = new List<SVector3>();
            List<SVector3> cols = new List<SVector3>();
            foreach (Transform t in transform)
            {
                if(t.TryGetComponent(out SpriteRenderer sprite))
                {
                    pos.Add(t.position);
                    cols.Add(new SVector3(sprite.color.r, sprite.color.g, sprite.color.b));
                }
            }
            return new SaveData
            {
                positions = pos,
                colors = cols
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            for (int i = 0; i < saveData.positions.Count; i++)
            {
                Color c = new Color(saveData.colors[i].x, saveData.colors[i].y, saveData.colors[i].z, 1);
                oozeContainer.SetOozeFromSave(c, saveData.positions[i]);
            }

        }

        [Serializable]
        private struct SaveData
        {
            public List<SVector3> positions;
            public List<SVector3> colors;
        }
    }

}