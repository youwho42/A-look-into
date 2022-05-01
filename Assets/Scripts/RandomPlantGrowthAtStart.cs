using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlantGrowthAtStart : MonoBehaviour
{

    public static RandomPlantGrowthAtStart instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        SetPlantGrowthAtStart();
    }
    public void SetPlantGrowthAtStart()
    {
        StartCoroutine(SetPlantGrowthAtStartCo());
    }

    IEnumerator SetPlantGrowthAtStartCo()
    {
        
        var allPlants = FindObjectsOfType<PlantGrowCycle>();
        foreach (var plant in allPlants)
        {
            
            plant.dayPlanted = Random.Range(-plant.plantCycles.Count+2, 3);
            plant.timeTickPlanted = Random.Range(300, 1000);
            plant.UpdateCycle(0);
            
            
        }
        yield return null;
    }

}
