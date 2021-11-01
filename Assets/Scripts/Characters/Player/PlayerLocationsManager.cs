using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocationsManager : MonoBehaviour
{
    
    public CurrentGridLocation currentLocation;

    Dictionary<Vector3Int, int> playerLocations = new Dictionary<Vector3Int, int>();

    private void FixedUpdate()
    {
        if (currentLocation.lastTilePosition != currentLocation.GetCurrentGridLocation())
        {
            AddLocation();
        }
    }

    void AddLocation()
    {

        currentLocation.lastTilePosition = currentLocation.GetCurrentGridLocation();
        if (!playerLocations.ContainsKey(currentLocation.lastTilePosition))
        {
            playerLocations.Add(currentLocation.lastTilePosition, 1);
        }
        else
        {
            playerLocations[currentLocation.lastTilePosition] ++;
        }
        if (playerLocations.TryGetValue(currentLocation.lastTilePosition, out int i))
            Debug.Log(currentLocation.lastTilePosition + " : " + i);
    }
}
