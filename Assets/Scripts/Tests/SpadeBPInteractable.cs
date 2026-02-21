using UnityEngine;
using System.Collections.Generic;


public class SpadeBPInteractable : SpadeInteractable
{
    public override void EndSpadeInteraction()
    {
        base.EndSpadeInteraction();
        if (TryGetComponent(out SpawnableBallPersonArea spawnableBallPerson))
            spawnableBallPerson.SpawnBP();
    }
}
