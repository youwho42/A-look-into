using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;
using Klaxon.SaveSystem;

public class RiverDebrisSpawner : MonoBehaviour
{

    public QI_ItemDatabase riverDebrisDatabase;
    public float spawnRadius;
    Dictionary<Transform, QI_ItemData> itemDicionary = new Dictionary<Transform, QI_ItemData>();
    private void Start()
    {
        Invoke("SpawnRandomItem", 1f);
    }
    void SpawnRandomItem()
    {
        
        var item = riverDebrisDatabase.GetRandomWeightedItem();
        var go = Instantiate(item.ItemPrefab, transform.position + GetRandomOffset(), Quaternion.identity);
        if (go.TryGetComponent(out SaveableItemEntity saveableItem))
            saveableItem.GenerateId();
        // reset next time a random item will spawn.
        float rand = Random.Range(1, 30);
        Invoke("SpawnRandomItem", rand);
    }

    Vector3 GetRandomOffset()
    {
        Vector3 r = Random.insideUnitCircle * spawnRadius;
        r.z = 0;
        return r;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
