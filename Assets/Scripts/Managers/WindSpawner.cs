using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WindSpawner : MonoBehaviour
{
    public static WindSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public Vector2 minMaxTimeToSpawn;
    float timeToSpawn;
    float nextTimeToSpawn;
    PlayerInformation player;
    ObjectPooler pool;
   

    private void Start()
    {
        pool = GetComponent<ObjectPooler>();
        player = PlayerInformation.instance;
        nextTimeToSpawn = Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
    }
    private void Update()
    {
        timeToSpawn += Time.deltaTime;
        if(timeToSpawn >= nextTimeToSpawn)
        {
            SpawnObject();
            timeToSpawn = 0;
            nextTimeToSpawn = Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
            
        }
    }

    private void SpawnObject()
    {
        GameObject go = pool.GetPooledObject();

        if(go != null)
        {
            go.transform.position = GetRandomPosition();
            CurrentGridLocation locationZ = go.GetComponent<CurrentGridLocation>();
            if (locationZ != null)
            {
                locationZ.GetTileLocation();
                locationZ.UpdateLocationAndPosition();
            }
            go.SetActive(true);
            IPoolPrefab np = go.GetComponent<IPoolPrefab>();
            if(np != null)
            {
                np.OnObjectSpawn();
                CheckAffectedObjects(go.transform);
            }
                
        }
    }

    private void CheckAffectedObjects(Transform location)
    {
        var hit = Physics2D.OverlapCircleAll(location.position, 1.5f);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if(hit[i].TryGetComponent(out IWindEffect affected))
                {
                    affected.Affect();
                }
            }
        }

    }

    Vector2 GetRandomPosition()
    {
        return (Random.insideUnitCircle * 2) + (Vector2)player.player.position;
    }
    
}
