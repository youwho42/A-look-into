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

    public bool playerActive = true;
    PlayerInformation player;
    public float minDist;
    public float maxDist;
    AudioSource audioSource;

    BoundsInt waterBounds;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(playerActive)
            player = PlayerInformation.instance;

        GetWaterTiles();
        
    }

    void GetWaterTiles()
    {
        var bounds = groundMap.cellBounds;
        if (bounds != waterBounds)
            waterBounds = bounds;
        else
            return;

        for (int x = 0; x < waterBounds.size.x; x++)
        {
            for (int y = 0; y < waterBounds.size.y; y++)
            {
                var px = waterBounds.xMin + x;
                var py = waterBounds.yMin + y;

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
            //GetWaterTiles();
            GetClosestDistanceFromPlayer();
            
            if (closestTileDistance < minDist)
            {
                audioSource.volume = 0.15f * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects);
            }
            else if (closestTileDistance > maxDist)
            {
                audioSource.volume = 0;
            }
            else
            {
                audioSource.volume = (0.4f - ((closestTileDistance - minDist) / (maxDist - minDist))) * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects); ;
            }
        }
        
    }

    void GetClosestDistanceFromPlayer()
    {
        closestTileDistance = 1000;
        for (int i = 0; i < spots.Count; i++)
        {
            Vector3 tile = groundGrid.CellToWorld(spots[i]);
            Vector3 pos = playerActive ? player.player.position : Camera.main.transform.position;
            float dist = Vector2.Distance(tile, pos);
            if (dist < closestTileDistance)
            {
                closestTileDistance = dist;
            }
        }
    }

}
