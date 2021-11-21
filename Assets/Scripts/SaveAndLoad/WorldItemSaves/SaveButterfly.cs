using QuantumTek.EncryptedSave;
using SerializableTypes;
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
        SVector3 location = transform.position;
        ES_Save.Save(location, ID);
        ES_Save.Save(flight.followingPlayer, ID + "following");
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
        flight.followingPlayer = ES_Save.Load<bool>(ID + "following");
        flight.GetNearestBase();
        flight.SetPosition(ES_Save.Load<SVector3>(ID));
        
    }
}
