using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlantGrowthAtStart : MonoBehaviour
{
    private void Start()
    {
        
        var allPlants = FindObjectsOfType<PlantLifeCycle>();
        foreach (var plant in allPlants)
        {
            
            plant.currentCycle = Random.Range(2, plant.plantCycles.Count);
            plant.currentTimeTick = Random.Range(400, 1200);
            plant.SetCurrentCycle();
        }
    }

}
