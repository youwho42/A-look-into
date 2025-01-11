using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using Klaxon.SaveSystem;
using Klaxon.Interactable;

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
        SetSpawnPoints();
        
        //SpawnObjects();
    }

    void SetSpawnPoints()
    {
        spawnPoints.Clear();
        Transform[] points = GetComponentsInChildren<Transform>();
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].CompareTag("ItemSpawnPoint"))
                spawnPoints.Add(points[i]);
        }
    }

    public void SpawnItemFromSave(int index, string name)
    {
        
        if (name != "")
        {
            
            var item = itemDatabase.GetItem(name);
            int i = Random.Range(0, item.ItemPrefabVariants.Count);
            var go = Instantiate(item.ItemPrefabVariants[i], spawnPoints[index].position, Quaternion.identity, spawnPoints[index]);
            //var go = Instantiate(item.ItemPrefabVariants[i], spawnPoints[index]);
            //go.localPosition = Vector3.zero;
            if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                Destroy(itemToSpawn);
        }
    }
    public List<string> GetSpawnedItems()
    {
        List<string> allItems = new List<string>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            string item = "";
            var child = spawnPoints[i].GetComponentInChildren<QI_Item>();
            if (child != null)
                item = child.Data.Name;
            allItems.Add(item);
        }
        
        return allItems;
    }

    public void SpawnObjects()
    {
        if (itemDatabase == null)
            return;
       
        
        foreach (var point in spawnPoints)
        {

            var child = point.GetComponentInChildren<Interactable>();
            if (child != null)
                continue;

            if (!GridManager.instance.GetTileValid(point.position))
                continue;

            if (Random.Range(0.0f, 1.0f) < chanceToSpawn)
            {

                var hit = Physics2D.OverlapCircle(point.position, minDistanceToSpawn);
                if (hit == null || hit.CompareTag("Grass") || hit.CompareTag("Animal") || hit.CompareTag("Path"))
                {

                    var item = itemDatabase.GetRandomWeightedItem();
                   
                    int i = Random.Range(0, item.ItemPrefabVariants.Count);
                    var go = Instantiate(item.ItemPrefabVariants[i], point.position, Quaternion.identity, point);

                    if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                        Destroy(itemToSpawn);
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
