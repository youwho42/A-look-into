using QuantumTek.QuantumAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsSaveSystem : MonoBehaviour, ISaveable
{
    public QA_AttributeHandler playerAttributes;

    public object CaptureState()
    {
        List<string> tempAttribute = new List<string>();
        List<float> tempValue = new List<float>();

        for (int i = 0; i < playerAttributes.Attributes.Count; i++)
        {
            tempAttribute.Add(playerAttributes.Attributes[i].Name);
            tempValue.Add(playerAttributes.Attributes[i].Value);
        }
        return new SaveData
        {
            attribute = tempAttribute,
            value = tempValue
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        for (int i = 0; i < playerAttributes.Attributes.Count; i++)
        {
            
            playerAttributes.SetAttributeValue(saveData.attribute[i], saveData.value[i]);
            
        }
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> attribute;
        public List<float> value;
    }
}
