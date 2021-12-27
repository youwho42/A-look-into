using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerLevelChange : MonoBehaviour
{

    public int playerLevel;

    public UnityEvent playerChangeLevelEvent;

    CurrentGridLocation currentLocation;

    private void Start()
    {
        currentLocation = GetComponent<CurrentGridLocation>();
        if (playerChangeLevelEvent == null)
        {
            playerChangeLevelEvent = new UnityEvent();
        }
        UpdatePlayerLocation();
    }

    private void Update()
    {
        
        currentLocation.UpdateLocation();
        

        if (playerLevel != currentLocation.currentLevel && Mathf.Abs(playerLevel - currentLocation.currentLevel) == 1)
        {
            playerLevel = currentLocation.currentLevel;
            Vector3 v = new Vector3(transform.position.x, transform.position.y, playerLevel);
            transform.position = v;
            playerChangeLevelEvent.Invoke();
        }
        
    }

    public void UpdatePlayerLocation()
    {
        currentLocation.UpdateLocation();
        playerLevel = currentLocation.currentLevel;
        playerChangeLevelEvent.Invoke();
    }


}
