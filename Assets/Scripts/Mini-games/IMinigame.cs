using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMinigame
{
    void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty);
    void SetupMiniGame(PokableItem pokable, MiniGameDificulty gameDificulty);
    void SetupMiniGame(SpadeJunkPileInteractor junkPile, MiniGameDificulty gameDificulty);
    void SetupMiniGame(SpadeInteractable spadeInteractable, MiniGameDificulty gameDificulty);
    void ResetMiniGame();
}
public enum MiniGameType
{
    None,
    Wood,
    Animal,
    Ore,
    Poking,
    Spade
}

public enum MiniGameDificulty
{
    None,
    Easy,
    Normal,
    Hard
}
