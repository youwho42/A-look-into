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
    public void SetPlantGrowthAtStart()
    {
        StartCoroutine(SetPlantGrowthAtStartCo());
    }

    IEnumerator SetPlantGrowthAtStartCo()
    {
        yield return new WaitForSeconds(1);
        var allPlants = FindObjectsOfType<PlantLifeCycle>();
        foreach (var plant in allPlants)
        {
            
            plant.currentCycle = Random.Range(2, plant.plantCycles.Count);
            plant.currentTimeTick = Random.Range(400, 1200);
            plant.SetCurrentCycle();
        }
    }

}
