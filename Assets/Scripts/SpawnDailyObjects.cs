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

    public bool canSpawnBeehive;
    [ConditionalHide("canSpawnBeehive", true)]
    public DrawZasYDisplacement beehivePlacement;
    [ConditionalHide("canSpawnBeehive", true)]
    public BeehiveObject beehiveObject;

    BeehiveObject currentBeehive;

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
        if (spawnPoints.Count > 0)
            return;
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
        foreach (Transform child in spawnPoints[index])
        {
            Destroy(child.gameObject);
        }
        if (name != "")
        {
            
            var item = itemDatabase.GetItem(name);
            if (item == null)
                Debug.Log(name, gameObject);
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

    public void SpawnObjects(float extraChance = 0.0f)
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

            
            if (Random.Range(0.0f, 1.0f) < chanceToSpawn + extraChance)
            {
                
                var hits = Physics2D.OverlapCircleAll(point.position, minDistanceToSpawn);
                bool canSpawn = true;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (!hits[i].CompareTag("Grass")
                        && !hits[i].CompareTag("Animal")
                        && !hits[i].CompareTag("Path")
                        && !hits[i].CompareTag("Minigame")
                        && !hits[i].CompareTag("Player"))
                    {
                        if (hits[i].TryGetComponent(out DrawZasYDisplacement displacement))
                        {
                            if (displacement.positionZ == 0)
                                continue;
                        }
                        canSpawn = false;
                        break;
                    }
                }
                
                if (!canSpawn)
                    return;

                
                var item = itemDatabase.GetRandomWeightedItem();

                int index = Random.Range(0, item.ItemPrefabVariants.Count);
                var go = Instantiate(item.ItemPrefabVariants[index], point.position, Quaternion.identity, point);

                if (go.TryGetComponent(out SaveableItemEntity itemToSpawn))
                    Destroy(itemToSpawn);
                   
            }
        }
    }

    
    public void ResetAtDawn()
    {
        SpawnObjects();
        if (TryGetComponent(out PokableItem pokable))
            pokable.SetTimesPoked(0);
        if(canSpawnBeehive)
            TrySpawnBeehive();
    }


    void TrySpawnBeehive()
    {
        if (currentBeehive != null)
        {
            currentBeehive.DestroyBeehive();
            currentBeehive = null;
        }
        float chance = 50.0f / ResetAtDawnManager.instance.allBeeTrees;
        if (Random.value < chance)
            SpawnBeehive();
    }

    void SpawnBeehive()
    {
        
        BeehiveObject beehive = Instantiate(beehiveObject);
        beehive.SetBeehive(beehivePlacement);
        currentBeehive = beehive;
    }
}
