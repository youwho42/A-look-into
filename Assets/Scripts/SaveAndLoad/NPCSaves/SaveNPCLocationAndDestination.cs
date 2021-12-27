using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.EncryptedSave;
using SerializableTypes;

public class SaveNPCLocationAndDestination : SaveableManager
{
    PathfindingGoToDestination pathfinding;

    private void Start()
    {
        pathfinding = GetComponent<PathfindingGoToDestination>();
    }

    public override void Save()
    {
        base.Save();
        SVector3 location = transform.position;
        SVector3 destination = pathfinding.currentDestination;
        ES_Save.Save(location, saveableName + "location");
        
        ES_Save.Save(pathfinding.nodesToDestination, saveableName + "nodes");
        ES_Save.Save(pathfinding.currentDestinationIndex, saveableName + "destinationIndex");
        ES_Save.Save(destination, saveableName + "destination");
        
        
    }

    public override void Load()
    {
        base.Load();

        StartCoroutine(LoadCo());
    }


    IEnumerator LoadCo()
    {
        
        base.Load();
        
        yield return new WaitForSeconds(0.015f);

        transform.position = ES_Save.Load<SVector3>(saveableName + "location");
        pathfinding.nodesToDestination = ES_Save.Load<List<Vector3>>(saveableName + "nodes");
        pathfinding.SetCurrentDestination(ES_Save.Load<SVector3>(saveableName + "destination"));
        pathfinding.currentDestinationIndex = ES_Save.Load<int>(saveableName + "destinationIndex");

        

    }
}
