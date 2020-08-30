using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

public class SpawnDailyObjects : MonoBehaviour
{

    public string objectToSpawn = "";

    
    public QI_ItemDatabase itemDatabase;

    public int hourToDailySpawn;

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
            if(hit == null)
            {
                var go = Instantiate(itemDatabase.GetItem(objectToSpawn).ItemPrefab, point.position, Quaternion.identity);
                if (go.TryGetComponent(out SaveableEntity entity))
                {
                    entity.GenerateId();
                }
            }
                
        }
    }
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }
}
