using QuantumTek.EncryptedSave;
using SerializableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSimpleWorldItem : SaveableItem
{

    public override void Save()
    {
        base.Save();
        SVector3 location = transform.position;
        ES_Save.Save(location, ID);

    }

    public override void Load()
    {
        base.Load();
        transform.position = ES_Save.Load<SVector3>(ID);
    }


    

}
