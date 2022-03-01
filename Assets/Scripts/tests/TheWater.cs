using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TheWater : MonoBehaviour
{


    
    public Transform waterBlob;
    public int waterAmount;
    List<Transform> allWaterBlobs = new List<Transform>();
    List<Vector3> directions = new List<Vector3>();
    List<float> speeds = new List<float>();
    public Vector3 initialDirection;
    public float speed;
    public float speedRange;
    public float spawnOffset;

    Grid groundGrid;
    Tilemap groundMap;


    private void Start()
    {
        SetGrid();
        for (int i = 0; i < waterAmount; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-spawnOffset, spawnOffset), Random.Range(-spawnOffset, spawnOffset));
            var go = Instantiate(waterBlob, transform.position + (Vector3)offset, Quaternion.identity);
            allWaterBlobs.Add(go);
            directions.Add(initialDirection);
            speeds.Add(speed + Random.Range(-speedRange, speedRange));
        }
    }

    private void Update()
    {
        MoveWaters();
    }
    void MoveWaters()
    {
        for (int i = 0; i < allWaterBlobs.Count; i++)
        {
            directions[i] = CanReachNextPosition(allWaterBlobs[i].position, directions[i]);
            allWaterBlobs[i].position = Move(allWaterBlobs[i], directions[i], speeds[i]);
           
        }
    }

    public Vector3 Move(Transform trans, Vector2 dir, float velocity)
    {

        var currentPosition = trans.position;
        
        currentPosition = Vector2.MoveTowards(trans.position, (Vector2)trans.position + dir, Time.deltaTime * velocity);

        return currentPosition;
        
        

    }


    public Vector3 CanReachNextPosition(Vector3 position, Vector3 direction)
    {



        float distance = 0.05f;
        Vector3 checkPosition = (position + direction * distance) - Vector3.forward;
        Vector3Int currentPosition = groundGrid.WorldToCell(position);
        Vector3Int nextTilePosition = groundGrid.WorldToCell(checkPosition);
        if (currentPosition == nextTilePosition - Vector3Int.forward)
            return direction;

        var tile = groundMap.GetTile(nextTilePosition);
        if (tile == null)
            return direction;

        Vector3Int diff = nextTilePosition - currentPosition;
        var newDirection = new Vector2(direction.y, direction.x);
        if (diff.x != 0)
            newDirection *= -1;

        return newDirection;


    }

    void SetGrid()
    {
        if (groundGrid != null)
            return;

        groundGrid = FindObjectOfType<Grid>();
        Tilemap[] maps = groundGrid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }

    }


}
