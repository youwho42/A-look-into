using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSaveLifeCycle : SaveableItem
{
    GrowingItem growingItem;

    private void Start()
    {
        growingItem = GetComponent<GrowingItem>();
    }

    public override void Save()
    {
        base.Save();
        ES_Save.SaveTransform(transform, ID);
        
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
        ES_Save.LoadTransform(transform, ID);
        yield return new WaitForSeconds(.01f);

        growingItem.SetCurrentTick(ES_Save.Load<int>(ID + "currentTimeTick"));
        
    }
}