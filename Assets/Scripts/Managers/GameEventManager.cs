using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public static class GameEventManager
{

    /// <summary>
    ///  Day Night cycle times, hours and ticks, maybe days later...
    /// </summary>
    public static readonly GameEvent<int> onTimeTickEvent = new GameEvent<int>();
    public static readonly GameEvent<int> onTimeHourEvent = new GameEvent<int>();
    public static readonly GameEvent<int> onShadowTickEvent = new GameEvent<int>();
    public static readonly GameEvent onDayStateChangeEvent = new GameEvent();

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
    public static readonly GameEvent onGuideCompediumUpdateEvent = new GameEvent();

    /// <summary>
    ///  Player Undertakings Update
    /// </summary>
    public static readonly GameEvent onUndertakingsUpdateEvent = new GameEvent();

    /// <summary>
    ///  World State Update
    /// </summary>
    public static readonly GameEvent onWorldStateUpdateEvent = new GameEvent();


    /// <summary>
    /// Game Started New or Load or Save
    /// </summary>
    public static readonly GameEvent onNewGameStartedEvent = new GameEvent();
    public static readonly GameEvent onGameStartLoadEvent = new GameEvent();
    public static readonly GameEvent onGameLoadedEvent = new GameEvent();
    public static readonly GameEvent onGameSavedEvent = new GameEvent();

    /// <summary>
    /// Scroll wheel scrolled
    /// </summary>
    public static readonly GameEvent<float> onMouseScrollEvent = new GameEvent<float>();

    /// <summary>
    /// UI interactions
    /// </summary>
    public static readonly GameEvent onEscapeEvent = new GameEvent();
    public static readonly GameEvent<GameObject> onEventSystemSelectedChangedEvent = new GameEvent<GameObject>();
    public static readonly GameEvent onMinigameMouseClickEvent = new GameEvent();
    public static readonly GameEvent onMenuToggleEvent = new GameEvent();
    public static readonly GameEvent onMenuDisplayEvent = new GameEvent();
    public static readonly GameEvent onMenuHideEvent = new GameEvent();
    public static readonly GameEvent onMapDisplayEvent = new GameEvent();
    public static readonly GameEvent onStackTransferGamepadEvent = new GameEvent();
    public static readonly GameEvent<int> onGamepadBumpersButtonEvent = new GameEvent<int>();
    public static readonly GameEvent<int> onGamepadTriggersButtonEvent = new GameEvent<int>();
    public static readonly GameEvent onInventoryDragEvent = new GameEvent();
    public static readonly GameEvent onInventoryRightClickEvent = new GameEvent();
    public static readonly GameEvent onInventoryRightClickReleaseEvent = new GameEvent();
    public static readonly GameEvent<string> onControlSchemeChangedEvent = new GameEvent<string>();
    public static readonly GameEvent onSubmitEvent = new GameEvent();
    public static readonly GameEvent onDialogueNextEvent = new GameEvent();
    public static readonly GameEvent onMapClickEvent = new GameEvent();

    /// <summary>
    /// Player Controls and animations
    /// </summary>
    public static readonly GameEvent onJumpEvent = new GameEvent();
    public static readonly GameEvent<float> onLandEvent = new GameEvent<float>();
    public static readonly GameEvent onUseEquipmentEvent = new GameEvent();
    public static readonly GameEvent<bool> onSpyglassAimEvent = new GameEvent<bool>();
    public static readonly GameEvent<int> onSpyglassAimChageSelectedEvent = new GameEvent<int>();
    public static readonly GameEvent onRotateDecoration = new GameEvent();
    public static readonly GameEvent onSurroundingItemsEvent = new GameEvent();
    public static readonly GameEvent onSmellEvent = new GameEvent();
    public static readonly GameEvent onSitEvent = new GameEvent();
    public static readonly GameEvent<bool> onExhaustedEvent = new GameEvent<bool>();
}