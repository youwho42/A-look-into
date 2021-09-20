using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpawner : MonoBehaviour
{
    ObjectPooler objectPool;
    PlayerInformation player;
    float timeToSpawn;
    private void Start()
    {
        player = PlayerInformation.instance;
        objectPool = ObjectPooler.instance;
        timeToSpawn = SetTimeToSpawn();
    }
    private void Update()
    {
        timeToSpawn -= Time.deltaTime;
        if (timeToSpawn <= 0)
        {
            objectPool.SpawnFromPool("Wind", SpawnPosition(), Quaternion.identity);
            timeToSpawn = SetTimeToSpawn();
        }
    }
    float SetTimeToSpawn()
    {
        return Random.Range(0.5f, 10.0f);
    }
    Vector3 SpawnPosition()
    {
        Vector3 pos = player.player.position + (Vector3)(Random.insideUnitCircle * 3);
        return pos;
    }
}
