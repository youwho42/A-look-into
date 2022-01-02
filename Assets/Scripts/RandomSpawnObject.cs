using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class RandomSpawnObject : MonoBehaviour
{
    public Grid groundGrid;
    public Tilemap groundMap;

    readonly float spriteDisplacementY = 0.27808595f;
    public List<QI_ItemData> objectsToSpawn;
    public int maxSpawnAmount;
    int quantity;
    public int timeToSpawn;
    public PolygonCollider2D spawnArea;

    private void Start()
    {
        DayNightCycle.instance.FullHourEventCallBack.AddListener(DailySpawnObjects);
    }
    public void DailySpawnObjects(int time)
    {
        if (time == timeToSpawn)
        {
            SpawnObjects();
        }
    }
    [ContextMenu("Spawn objects")]
    public void SpawnObjects()
    {
        int amount = maxSpawnAmount - WorldItemManager.instance.GetWorldObjectAmount();
        quantity = 0;
        while (quantity < amount)
        {
            PlaceObject();
        }
    }

    [ContextMenu("Clear objects from world dictionary")]
    public void ClearObjects()
    {
        WorldItemManager.instance.RemoveAllItemsFromWorldItemDictionary();
    }
    
    public void PlaceObject()
    {
        Vector3Int temp = GetRandomTilePosition();
        if (temp.z >= 1)
        {
            var hit = Physics2D.OverlapCircle(SetPositionY(temp), .01f);
            if (hit == null)
            {
                int rand = UnityEngine.Random.Range(0, objectsToSpawn.Count);
                var go = Instantiate(objectsToSpawn[rand].ItemPrefab, SetPositionY(temp), Quaternion.identity);
                go.transform.parent = this.transform;
                if (go.TryGetComponent(out SaveableItemEntity entity))
                {
                    entity.GenerateId();
                }
                WorldItemManager.instance.AddItemToWorldItemDictionary(objectsToSpawn[rand].Name, 1);
                quantity++;
            }
        }


    }
    Vector3 SetPositionY(Vector3Int currentPosition)
    {
        Vector3 newPosition = groundMap.GetCellCenterWorld(currentPosition);
        Vector3 displacement = new Vector3(UnityEngine.Random.Range(-.2f, .2f), UnityEngine.Random.Range(-.2f, .2f),0);
        return newPosition + displacement;
    }

    int GetTileLocation(Vector3Int currentPosition)
    {
        int tilesHit = 0;

        for (int i = 0; i < groundMap.size.z; i++)
        {
            currentPosition.z = i;
            TileBase tile = groundMap.GetTile(currentPosition);
            if (tile != null)
            {
                tilesHit++;
            }
        }
        return tilesHit;
    }

    

    Vector3Int GetRandomTilePosition()
    {
        Vector3Int rand = new Vector3Int(UnityEngine.Random.Range(groundMap.cellBounds.xMin+5, groundMap.cellBounds.xMax-5), UnityEngine.Random.Range(groundMap.cellBounds.yMin+6, groundMap.cellBounds.yMax-6), 0);
        
        rand.z = GetTileLocation(rand);
        
        return rand;
    }

    
}
