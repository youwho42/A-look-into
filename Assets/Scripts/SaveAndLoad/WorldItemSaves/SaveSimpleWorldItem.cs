using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSimpleWorldItem : SaveableItem
{

    public override void Save()
    {
        base.Save();
        ES_Save.SaveTransform(transform, ID);

    }

    public override void Load()
    {
        base.Load();
        ES_Save.LoadTransform(transform, ID);
    }


    

}
