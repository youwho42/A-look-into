using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class SpawnDailyObjects : MonoBehaviour
{

    public List<QI_ItemData> objectToSpawn = new List<QI_ItemData>();

    
    

    int hourToDailySpawn = 5;

    List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        Transform[] points = GetComponentsInChildren<Transform>();
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].CompareTag("ItemSpawnPoint"))
                spawnPoints.Add(points[i]);
        }
        GameEventManager.onTimeHourEvent.AddListener(SpawnObjects);
        SpawnObjects(hourToDailySpawn);
    }
    public void SpawnObjects(int timeOfDay)
    {
        if (timeOfDay != hourToDailySpawn)
            return;
        if(TryGetComponent(out PlantGrowCycle plantCycle))
        {
            if (plantCycle.currentCycle < plantCycle.gatherableCycle)
                return;
        }
        foreach (var point in spawnPoints)
        {
            if (Random.Range(0.0f, 1.0f) > 0.3f)
            {
                var hit = Physics2D.OverlapCircle(point.position, .05f);
                if (hit == null || hit.CompareTag("Grass"))
                {

                    int r = Random.Range(0, objectToSpawn.Count);

                    var go = Instantiate(objectToSpawn[r].ItemPrefab, point.position, Quaternion.identity);


                    if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                    {
                        itemToSpawn.GenerateId();
                    }
                }
            }
        }
    }
    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SpawnObjects);
    }
}
