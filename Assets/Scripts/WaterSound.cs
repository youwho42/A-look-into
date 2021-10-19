using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSound : MonoBehaviour
{
    public Grid groundGrid;
    public Tilemap groundMap;

    float closestTileDistance=1000;

    List<Vector3Int> spots = new List<Vector3Int>();

    PlayerInformation player;
    public float minDist;
    public float maxDist;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = PlayerInformation.instance;
        groundMap.CompressBounds();
        var bounds = groundMap.cellBounds;
        

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                var px = bounds.xMin + x;
                var py = bounds.yMin + y;

                if (groundMap.HasTile(new Vector3Int(px, py, 0)))
                {
                    spots.Add(new Vector3Int(px, py, 0));
                }
                
            }
        }
        
    }

    private void Update()
    {
        if (Time.frameCount % 33 == 0)
        {
            GetClosestDistanceFromPlayer();
            
            if (closestTileDistance < minDist)
            {
                audioSource.volume = 0.2f;
            }
            else if (closestTileDistance > maxDist)
            {
                audioSource.volume = 0;
            }
            else
            {
                audioSource.volume = 0.2f - ((closestTileDistance - minDist) / (maxDist - minDist));
            }
        }
        
    }

    void GetClosestDistanceFromPlayer()
    {
        closestTileDistance = 1000;
        for (int i = 0; i < spots.Count; i++)
        {
            Vector3 tile = groundGrid.CellToWorld(spots[i]);
            
            float dist = Vector2.Distance(tile, player.player.position);
            if (dist < closestTileDistance)
            {
                closestTileDistance = dist;
            }
        }
    }

}
