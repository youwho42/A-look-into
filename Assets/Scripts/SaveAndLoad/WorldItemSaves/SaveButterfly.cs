using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButterfly : SaveableItem
{
    EntityReproduction reproduction;
    CharacterFlight flight;
    private void Start()
    {
        reproduction = GetComponent<EntityReproduction>();
        flight = GetComponent<CharacterFlight>();

    }

    public override void Save()
    {
        base.Save();
        ES_Save.Save(reproduction.GetTick(), ID + "tick");
        ES_Save.Save(transform.position.x, ID + "areaX");
        ES_Save.Save(transform.position.y, ID + "areaY");
        ES_Save.Save(transform.position.z, ID + "areaZ");
        
    }
    public override void Load()
    {
        base.Load();
        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {

        base.Load();
        reproduction = GetComponent<EntityReproduction>();
        flight = GetComponent<CharacterFlight>();

        yield return new WaitForSeconds(0.1f);
        reproduction.SetTick(ES_Save.Load<int>(ID + "tick"));
        float x = ES_Save.Load<float>(ID + "areaX");
        float y = ES_Save.Load<float>(ID + "areaY");
        float z = ES_Save.Load<float>(ID + "areaZ");
        flight.SetPosition(new Vector3(x, y, z));
        
    }
}
