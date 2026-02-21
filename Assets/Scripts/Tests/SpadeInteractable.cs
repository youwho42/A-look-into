using UnityEngine;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using System;

[Serializable]
public class SpadeInteractable : MonoBehaviour
{
    public EquipmentTier spadeInteractionTier;
    public QI_ItemDatabase spadeInteractionDatabase;
    public MiniGameDificulty gameDificulty;
    public Collider2D interactCollider;
    public ReplaceObjectOnItemDrop replaceObjectOnDrop;
    [HideInInspector]
    public bool hasInteracted;

    public virtual void EndSpadeInteraction()
    {
        hasInteracted = true;
    }

    private void Start()
    {
        GameEventManager.onGameLoadedEvent.AddListener(CheckForObjectsToHide);
        GameEventManager.onGameStartLoadEvent.AddListener(CheckForObjectsToHide);
    }
    private void OnDisable()
    {
        GameEventManager.onGameLoadedEvent.RemoveListener(CheckForObjectsToHide);
        GameEventManager.onGameStartLoadEvent.RemoveListener(CheckForObjectsToHide);
    }
    void CheckForObjectsToHide()
    {
        if (replaceObjectOnDrop == null)
            return;
        replaceObjectOnDrop.ShowObjects(true);
        replaceObjectOnDrop.CheckForObjects();
    }
}
