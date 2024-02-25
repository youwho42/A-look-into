using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.SaveSystem
{
    public class SaveableItemEntity : MonoBehaviour
    {
        [SerializeField]
        private string id = string.Empty;

        public string ID => id;

        
        public void GenerateId() => id = Guid.NewGuid().ToString();


        public string version;

        [ContextMenu("Set ID and Version")]
        public void SetVersion()
        {
            version = Application.version;
            GenerateId();
        }
        public void SetVersionFromSave(string savedVersion)
        {
            version = savedVersion;
        }

        public void SetId(string newID)
        {
            id = newID;
        }

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();

            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var stateDictionary = (Dictionary<string, object>)state;

            foreach (var saveable in GetComponents<ISaveable>())
            {
                string typeName = saveable.GetType().ToString();

                if (stateDictionary.TryGetValue(typeName, out object value))
                {
                    saveable.RestoreState(value);
                }
            }
        }
    } 
}
