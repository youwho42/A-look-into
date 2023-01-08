using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventManager
{

    /// <summary>
    ///  Day Night cycle times, hours and ticks, maybe days later...
    /// </summary>
    public static readonly GameEvent<int> onTimeTickEvent = new GameEvent<int>();
    public static readonly GameEvent<int> onTimeHourEvent = new GameEvent<int>();


    /// <summary>
    ///  Inventory and Equipment Updating
    /// </summary>
    public static readonly GameEvent onInventoryUpdateEvent = new GameEvent();
    public static readonly GameEvent onEquipmentUpdateEvent = new GameEvent();

    /// <summary>
    ///  Stats Updating
    /// </summary>
    public static readonly GameEvent onStatUpdateEvent = new GameEvent();

    /// <summary>
    ///  Player Position Update
    /// </summary>
    public static readonly GameEvent onPlayerPositionUpdateEvent = new GameEvent();

    /// <summary>
    ///  Player Compendiums Update
    /// </summary>
    public static readonly GameEvent onAnimalCompediumUpdateEvent = new GameEvent();
    public static readonly GameEvent onResourceCompediumUpdateEvent = new GameEvent();
    public static readonly GameEvent onRecipeCompediumUpdateEvent = new GameEvent();
    public static readonly GameEvent onNoteCompediumUpdateEvent = new GameEvent();

    /// <summary>
    ///  Player Undertakings Update
    /// </summary>
    public static readonly GameEvent onUndertakingsUpdateEvent = new GameEvent();


    /// <summary>
    /// Game Started New or Load or Save
    /// </summary>
    public static readonly GameEvent onNewGameStartedEvent = new GameEvent();
    public static readonly GameEvent onGameLoadedEvent = new GameEvent();
    public static readonly GameEvent onGameSavedEvent = new GameEvent();
}