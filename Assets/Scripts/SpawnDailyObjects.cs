using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using Klaxon.SaveSystem;

public class SpawnDailyObjects : MonoBehaviour, IResetAtDawn
{

    public QI_ItemDatabase itemDatabase;

    public bool hideOnStart;
    
    List<Transform> spawnPoints = new List<Transform>();

    public float minDistanceToSpawn = 0.05f;
    [Range(0f, 1f)]
    public float chanceToSpawn = 0.2f;
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
        
        //SpawnObjects();
    }


    public void SpawnObjects()
    {
        if (itemDatabase == null)
            return;
       
        if (TryGetComponent(out PlantGrowCycle plantCycle))
        {
            if (plantCycle.currentCycle < plantCycle.gatherableCycle)
                return;
        }
        foreach (var point in spawnPoints)
        {
            if (!GridManager.instance.GetTileValid(point.position))
                continue;
            if (Random.Range(0.0f, 1.0f) < chanceToSpawn)
            {

                var hit = Physics2D.OverlapCircle(point.position, minDistanceToSpawn);
                if (hit == null || hit.CompareTag("Grass") || hit.CompareTag("Animal") || hit.CompareTag("Path"))
                {
                    
                    var item = itemDatabase.GetRandomWeightedItem();
                    var go = Instantiate(item.ItemPrefabVariants[0], point.position, Quaternion.identity, transform);

                    if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                        itemToSpawn.GenerateId();
                }
            }
        }
    }

    
    public void ResetAtDawn()
    {
        SpawnObjects();
        if (TryGetComponent(out PokableItem pokable))
            pokable.SetTimesPoked(0);
    }

}
