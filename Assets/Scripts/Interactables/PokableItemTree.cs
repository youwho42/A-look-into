using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItemTree : PokableItem
{

    //public QI_ItemDatabase treeItemDatabase;


    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        if (TryGetComponent(out TreeRustling rustling))
            rustling.Affect(true);
        if (TryGetComponent(out SpawnDailyObjects spawner) && TimesPoked <= 5)
            spawner.SpawnObjects(0.3f);
        
    }
    public override void PokeItemFail()
    {
        base.PokeItemFail();
        if (TryGetComponent(out TreeRustling rustling))
            rustling.Affect(false);
    }
}
