using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnableBallPersonArea : MonoBehaviour
{

    [HideInInspector]
    public bool hasSpawned;
    public SpriteRenderer marker;
    //public Transform spawnPosition;
    public bool GetHasSpawned()
    {
        return hasSpawned;
    }
    public void SetHasSpawned(bool _hasSpawned)
    {
        hasSpawned = _hasSpawned;
        marker.enabled = !hasSpawned;
    }
}
