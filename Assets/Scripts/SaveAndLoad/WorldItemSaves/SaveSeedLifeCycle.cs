using QuantumTek.EncryptedSave;
using SerializableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSeedLifeCycle : SaveableItem
{
    GrowingItem growingItem;

    private void Start()
    {
        growingItem = GetComponent<GrowingItem>();
    }

    public override void Save()
    {
        base.Save();
        SVector3 location = transform.position;
        ES_Save.Save(location, ID);

        ES_Save.Save(growingItem.GetCurrentTick(), ID + "currentTimeTick");

    }

    public override void Load()
    {
        base.Load();
        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {
        growingItem = GetComponent<GrowingItem>();
        transform.position = ES_Save.Load<SVector3>(ID);
        yield return new WaitForSeconds(.01f);

        growingItem.SetCurrentTick(ES_Save.Load<int>(ID + "currentTimeTick"));
        
    }
}