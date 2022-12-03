using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMinigame
{
    void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty);
    void ResetMiniGame();
}
public enum MiniGameType
{
    None,
    Wood,
    Animal,
    Ore,
    Fixing
}

public enum MiniGameDificulty
{
    None,
    Easy,
    Normal,
    Hard
}
