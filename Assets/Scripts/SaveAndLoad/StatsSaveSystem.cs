using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumAttributes;
using System;

public class StatsSaveSystem : MonoBehaviour, ISaveable
{

    public QA_AttributeHandler playerAttributes;

    public object CaptureState()
    {
        List<string> tempString = new List<string>();
        List<float> tempFloat = new List<float>();
        for (int i = 0; i < playerAttributes.Attributes.Count; i++)
        {
            tempString.Add(playerAttributes.Attributes[i].Name);
            tempFloat.Add(playerAttributes.Attributes[i].Value);
        }

        return new SaveData
        {
            attributeName = tempString,
            attributeAmount = tempFloat
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        
        for (int i = 0; i < saveData.attributeAmount.Count; i++)
        {
            playerAttributes.SetAttributeValue(saveData.attributeName[i], saveData.attributeAmount[i]);

        }
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> attributeName;
        public List<float> attributeAmount;


    }

}
