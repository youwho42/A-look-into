using QuantumTek.EncryptedSave;
using SerializableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SavePlantLifeCycle : SaveableItem
{
    PlantLifeCycle plantLifeCycle;

    public virtual void Start()
    {
        plantLifeCycle = GetComponent<PlantLifeCycle>();
    }

    public override void Save()
    {
        base.Save();
        SVector3 location = transform.position;
        ES_Save.Save(location, ID);
        ES_Save.Save(plantLifeCycle.currentCycle, ID + "currentCycle");
        ES_Save.Save(plantLifeCycle.currentTimeTick, ID + "currentTimeTick");
        ES_Save.Save(plantLifeCycle.homeOccupiedBy, ID + "homeOccupiedBy");

    }

    public override void Load()
    {
        base.Load();
        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {
        plantLifeCycle = GetComponent<PlantLifeCycle>();
        transform.position = ES_Save.Load<SVector3>(ID);

        yield return new WaitForSeconds(.01f);
        
        plantLifeCycle.currentCycle = ES_Save.Load<int>(ID + "currentCycle");
        plantLifeCycle.currentTimeTick = ES_Save.Load<int>(ID + "currentTimeTick");
        plantLifeCycle.homeOccupiedBy = ES_Save.Load<string>(ID + "homeOccupiedBy");
        plantLifeCycle.SetCurrentCycle();
        plantLifeCycle.SetHomeOccupation();
    }
}
