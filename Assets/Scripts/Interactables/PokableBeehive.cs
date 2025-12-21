using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using Klaxon.SaveSystem;


public class PokableBeehive : PokableItem
{
    public QI_ItemData spawnItem;
    public BeehiveObject beehiveObject;

    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        var go = Instantiate(spawnItem.ItemPrefabVariants[0], transform.position, Quaternion.identity);
        if(go.TryGetComponent(out SaveableItemEntity saveable))
            saveable.GenerateId();

        beehiveObject.FadeAll();
        this.enabled = false;
    }
    public override void PokeItemFail()
    {
        base.PokeItemFail();
        beehiveObject.FadeAll();
        this.enabled = false;
    }
}