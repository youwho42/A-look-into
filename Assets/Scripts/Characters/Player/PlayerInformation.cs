using QuantumTek.QuantumInventory;
using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
using System;
using Klaxon.QuestSystem;
using Klaxon.UndertakingSystem;
using UnityEngine.U2D.Animation;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation instance;

    public Transform player;
    public SpriteRenderer playerSprite;
    public GameObject playerShadow;
    public GravityItemMovementControllerNew playerController;
    public QI_Inventory playerInventory;
    public EquipmentManager equipmentManager;
    public QI_CraftingRecipeDatabase playerRecipeDatabase;
    public QI_ItemDatabase playerResourceCompendiumDatabase;
    public QI_ItemDatabase playerAnimalCompendiumDatabase;
    public QI_ItemDatabase playerNotesCompendiumDatabase;
    public QI_ItemDatabase playerGuidesCompendiumDatabase;
    public bool uiScreenVisible;
    
    public PlayerInputController playerInput;
    public PlayerActivateSpyglass playerActivateSpyglass;

    public PlayerStats playerStats;
    public CurrentTilePosition currentTilePosition;
    public Animator playerAnimator;

    public PlayerAnimalCompendiumInformation animalCompendiumInformation;
    public PlayerCharacterManager characterManager;

    public PlayerUndertakingHandler playerUndertakings;

    public string playerName { get; private set; }

    public SpriteResolver playerSpriteResolver;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    private void Start()
    {
        currentTilePosition = player.GetComponent<CurrentTilePosition>();
        playerAnimalCompendiumDatabase.Items.Clear();
        playerResourceCompendiumDatabase.Items.Clear();
        playerNotesCompendiumDatabase.Items.Clear();
        playerGuidesCompendiumDatabase.Items.Clear();
        GameEventManager.onInventoryUpdateEvent.AddListener(UpdatePlayerResources);
    }

    private void OnDestroy()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(UpdatePlayerResources);
    }
    public void TogglePlayerInput(bool toggle)
    {
        playerInput.isInUI = !toggle;
    }

    public int GetTotalInventoryQuantity(QI_ItemData item)
    {
        int amountInInventory = 0;
        for (int i = 0; i < playerInventory.Stacks.Count; i++)
        {
            if(playerInventory.Stacks[i].Item == item)
            {
                amountInInventory += playerInventory.Stacks[i].Amount;
            }
        }
        return amountInInventory;
    }

    void UpdatePlayerResources()
    {
        foreach (var item in playerInventory.Stacks)
        {
            if (!playerResourceCompendiumDatabase.Items.Contains(item.Item))
            {
                playerResourceCompendiumDatabase.Items.Add(item.Item);
                
                AddGuideToCompendium(item.Item);
                NotificationManager.instance.SetNewNotification($"{item.Item.Name} added to items", NotificationManager.NotificationType.Compendium);
                GameEventManager.onResourceCompediumUpdateEvent.Invoke();
            }
        }
        
    }

    void AddGuideToCompendium(QI_ItemData item)
    {
        if (item.compendiumGuide != null)
        {
            if (!playerGuidesCompendiumDatabase.Items.Contains(item.compendiumGuide))
            {
                playerGuidesCompendiumDatabase.Items.Add(item.compendiumGuide);
                NotificationManager.instance.SetNewNotification($"{item.compendiumGuide.Name} added to guides", NotificationManager.NotificationType.Compendium);
            }
        }
            
    }
}
