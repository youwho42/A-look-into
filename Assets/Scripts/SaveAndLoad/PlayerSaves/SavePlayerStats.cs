using QuantumTek.EncryptedSave;
using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerStats : SaveableManager
{
    public QA_AttributeHandler playerAttributes;

    List<string> attribute = new List<string>();
    List<float> value = new List<float>();

    private void Start()
    {
        
    }

    public override void Save()
    {
        base.Save();
        attribute.Clear();
        value.Clear();

        for (int i = 0; i < playerAttributes.Attributes.Count; i++)
        {
            attribute.Add(playerAttributes.Attributes[i].Name);
            value.Add(playerAttributes.Attributes[i].Value);
        }
        ES_Save.Save(attribute, saveableName + "attribute");
        ES_Save.Save(value, saveableName + "value");
    }

    public override void Load()
    {
        base.Load();
        attribute.Clear();
        value.Clear();
        attribute = ES_Save.Load<List<string>>(saveableName + "attribute");
        value = ES_Save.Load<List<float>>(saveableName + "value");
        for (int i = 0; i < attribute.Count; i++)
        {
            playerAttributes.SetAttributeValue(attribute[i], value[i]);
        }
    }

}
