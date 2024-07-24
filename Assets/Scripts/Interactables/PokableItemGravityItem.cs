using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItemGravityItem : PokableItem
{


    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        Vector3 direction = transform.position - PlayerInformation.instance.player.position;
        if (TryGetComponent(out GravityItemMovementFree item))
            item.AddMovement(direction, 0.7f);
    }
    public override void PokeItemFail()
    {
        base.PokeItemFail();

    }
}
