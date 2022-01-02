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
        DayNightCycle.instance.FullHourEventCallBack.AddListener(SpawnObjects);
    }
    public void SpawnObjects(int timeOfDay)
    {
        if (timeOfDay != hourToDailySpawn)
            return;
        foreach (var point in spawnPoints)
        {
            var hit = Physics2D.OverlapCircle(point.position, .05f);
            if(hit == null || hit.CompareTag("Grass"))
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
    private void OnDestroy()
    {
        DayNightCycle.instance.FullHourEventCallBack.RemoveListener(SpawnObjects);
    }
}
