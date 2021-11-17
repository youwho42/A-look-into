using QuantumTek.EncryptedSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveReproductivePlantCycle : SavePlantLifeCycle
{

    EntityReproduction reproduction;

    public override void Start()
    {
        base.Start();
        reproduction = GetComponent<EntityReproduction>();
    }
    public override void Save()
    {
        base.Save();
        ES_Save.Save(reproduction.GetTick(), ID + "tick");

    }

    public override void Load()
    {
        reproduction = GetComponent<EntityReproduction>();
        base.Load();
        reproduction.SetTick(ES_Save.Load<int>(ID + "tick"));

    }

}
