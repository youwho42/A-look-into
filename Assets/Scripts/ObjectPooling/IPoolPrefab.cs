using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolPrefab
{
    void OnObjectSpawn();

    void DeactivateObject();
}
