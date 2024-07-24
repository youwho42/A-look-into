using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItemRock : PokableItem
{

    

    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        
        if (TryGetComponent(out SpawnDailyObjects spawner) && TimesPoked <= 5)
            spawner.SpawnObjects();
    }
    public override void PokeItemFail()
    {
        base.PokeItemFail();
        
    }
}
