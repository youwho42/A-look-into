using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class SpawnDailyObjects : MonoBehaviour
{

    public QI_ItemDatabase itemDatabase;

    public bool hideOnStart;
    
    int hourToDailySpawn = 5;

    List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        
        if(gameObject.TryGetComponent(out SpriteRenderer rend) && hideOnStart)
        {
            rend.enabled = false;
        }
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
        if (timeOfDay != hourToDailySpawn || itemDatabase == null)
            return;
       
        if (TryGetComponent(out PlantGrowCycle plantCycle))
        {
            if (plantCycle.currentCycle < plantCycle.gatherableCycle)
                return;
        }
        foreach (var point in spawnPoints)
        {
            if (Random.Range(0.0f, 1.0f) > 0.6f)
            {
                var hit = Physics2D.OverlapCircle(point.position, .05f);
                if (hit == null || hit.CompareTag("Grass"))
                {
                    
                    var item = itemDatabase.GetRandomWeightedItem();
                    var go = Instantiate(item.ItemPrefab, point.position, Quaternion.identity);

                    if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                        itemToSpawn.GenerateId();
                }
            }
        }
    }

    

    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SpawnObjects);
    }
}
