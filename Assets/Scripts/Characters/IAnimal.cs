using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimal
{
    public void FleePlayer(Transform playerTransform);
    public void SetActiveState(bool active);
}
