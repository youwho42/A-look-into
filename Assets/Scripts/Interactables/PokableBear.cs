using UnityEngine;
using System.Collections.Generic;


public class PokableBear : PokableItem
{

    

    private void OnEnable()
    {
        GameEventManager.onPokableMinigameStartEvent.AddListener(IsPoking);
    }

    private void OnDisable()
    {
        GameEventManager.onPokableMinigameStartEvent.RemoveListener(IsPoking);
    }

    void IsPoking(PokableItem pokable)
    {
        if (pokable == this)
            Debug.Log("Le Poke!");
    }

    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        
        if (SuccessfulTimesPoked == 1)
            Debug.Log("warn");
        if (SuccessfulTimesPoked == 2)
            Debug.Log("empty inventory");
        if (SuccessfulTimesPoked >= 3)
            Debug.Log("delete save");
        
    }

    public override void PokeItemFail()
    {
        base.PokeItemFail();
    }

}
