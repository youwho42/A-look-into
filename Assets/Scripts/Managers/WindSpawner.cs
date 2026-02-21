using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    
    public Transform objectToFollow;
    ObjectPooler pool;
    PlayerInformation player;
    GridManager grid;
    float minWindMagnitude = 2.0f;

    private void Start()
    {
        player = PlayerInformation.instance;
        grid = GridManager.instance;
        pool = GetComponent<ObjectPooler>();
        GameEventManager.onTimeTickEvent.AddListener(CheckTilesForWind);
    }

    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(CheckTilesForWind);
    }

    void CheckTilesForWind(int tick)
    {
        int dist = 5;
        for (int x = -dist; x < dist; x++) 
        {
            for (int y = -dist; y < dist; y++)
            {
                Vector2Int intPos = (Vector2Int)player.currentTilePosition.position;
                intPos.x += x;
                intPos.y += y;
                
                if(grid.TryGetTileValid(intPos, out Vector3Int tilePos))
                {
                    var pos = grid.GetTileWorldPosition(tilePos);
                    var screenPos = Camera.main.WorldToScreenPoint(pos);
                    var onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
                    if (onScreen)
                        TryStartWind(tick, pos, tilePos);
                }
            }
        }
    }

    

    void TryStartWind(int tick, Vector3 position, Vector3Int intPos)
    {
       
        if (CheckLocationIndex(tick, intPos))
        {
            float m = WindManager.instance.GetWindMagnitude(position);
            bool chance = Random.value < m * 0.05f;
            if (m > minWindMagnitude && chance)
                SpawnObject(position);
        }
    }

    bool CheckLocationIndex(int tick, Vector3Int intPos)
    {
        int phase = intPos.x * 73856093 ^ intPos.y * 19349663;
        phase &= 0x7fffffff;

        int interval = 6;
        return ((tick + phase) % interval) == 0;


    }


    private void SpawnObject(Vector3 position)
    {
        GameObject go = pool.GetPooledObject();

        if(go != null)
        {
            go.transform.position = position + (Vector3)GetRandomPosition();
            CurrentGridLocation locationZ = go.GetComponent<CurrentGridLocation>();
            if (locationZ != null)
            {
                locationZ.GetTileLocation();
                locationZ.UpdateLocationAndPosition();
            }
            
            go.SetActive(true);
            var d = WindManager.instance.GetWindDirectionFromPosition(position);
            var dir = d.x < 0 ? Vector3.one : new Vector3(-1,1,1);
            go.transform.localScale = dir;
            IPoolPrefab np = go.GetComponent<IPoolPrefab>();
            
            if(np != null)
                np.OnObjectSpawn();
        }
    }


    Vector2 GetRandomPosition()
    {
        return (Random.insideUnitCircle * 0.3f);
    }

}
