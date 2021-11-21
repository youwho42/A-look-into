using QuantumTek.EncryptedSave;
using SerializableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerLocation : SaveableManager
{

    PlayerLevelChange playerLevel;

    private void Start()
    {
        playerLevel = GetComponent<PlayerLevelChange>();
    }

    public override void Save()
    {
        base.Save();


        SVector3 location = transform.position;
        
        ES_Save.Save(location, saveableName + "SVector");
    }

    public override void Load()
    {
        base.Load();

        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {
        playerLevel ??= GetComponent<PlayerLevelChange>();
        base.Load();
        
        yield return new WaitForSeconds(0.015f);

        transform.position = ES_Save.Load<SVector3>(saveableName + "SVector");
        playerLevel.UpdatePlayerLocation();
    }
}
