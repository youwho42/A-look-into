using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerLocation : SaveableManager
{
    public override void Save()
    {
        base.Save();
        

        ES_Save.Save(transform.position.x, saveableName + "LocationX");
        ES_Save.Save(transform.position.y, saveableName + "LocationY");
        ES_Save.Save(transform.position.z, saveableName + "LocationZ");

    }

    public override void Load()
    {
        base.Load();
        float x = ES_Save.Load<float>(saveableName + "LocationX");
        float y = ES_Save.Load<float>(saveableName + "LocationY");
        float z = ES_Save.Load<float>(saveableName + "LocationZ");
        transform.position = new Vector3(x, y, z);
        FindObjectOfType<PlayerLevelChange>().UpdatePlayerLocation();
    }
}
