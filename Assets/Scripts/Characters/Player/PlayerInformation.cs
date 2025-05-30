using QuantumTek.QuantumInventory;
using UnityEngine;
using Klaxon.GravitySystem;
using Klaxon.UndertakingSystem;
using UnityEngine.U2D.Animation;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using Klaxon.StatSystem;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation instance;

    public Transform player;
    public SpriteRenderer playerSprite;
    public GameObject playerShadow;
    public GravityItemMovementControllerNew playerController;
    public AnimatePlayer animatePlayerScript;
    public Transform playerPokableSpot;
    public PlayerPoke playerPoke;
    public PlayerRunningManager runningManager;


    public QI_Inventory playerInventory;
    public EquipmentManager equipmentManager;
    public QI_CraftingRecipeDatabase playerRecipeDatabase;
    public QI_ItemDatabase playerResourceCompendiumDatabase;
    public QI_ItemDatabase playerEncountersCompendiumDatabase;
    public QI_ItemDatabase playerAnimalCompendiumDatabase;
    public QI_ItemDatabase playerNotesCompendiumDatabase;
    public QI_ItemDatabase playerGuidesCompendiumDatabase;
    public PlayerPurse purse;
    public bool uiScreenVisible;
    
    public PlayerInputController playerInput;
    public PlayerActivateSpyglass playerActivateSpyglass;

    public CurrentTilePosition currentTilePosition;
    public Animator playerAnimator;

    public PlayerAnimalCompendiumInformation animalCompendiumInformation;
    public PlayerCharacterManager characterManager;

    public PlayerUndertakingHandler playerUndertakings;

    public bool isSitting;
    public string playerName { get; private set; }
    StringVariable pName;
    public SpriteResolver playerSpriteResolver;
    public StatHandler statHandler;
    public bool isDragging;
    public InventoryDisplaySlot inventorySlot;
    public bool inMaze;

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
        pName.Value = playerName;
    }

    private void Start()
    {
        currentTilePosition = player.GetComponent<CurrentTilePosition>();
        playerEncountersCompendiumDatabase.Items.Clear();
        playerAnimalCompendiumDatabase.Items.Clear();
        playerResourceCompendiumDatabase.Items.Clear();
        playerNotesCompendiumDatabase.Items.Clear();
        playerGuidesCompendiumDatabase.Items.Clear();
        GameEventManager.onInventoryUpdateEvent.AddListener(UpdatePlayerResources);
        var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        pName = source["global"]["playerName"] as StringVariable;
        
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
                
                Notifications.instance.SetNewLargeNotification(null, item.Item, null, NotificationsType.Compendium);
                //Notifications.instance.SetNewLargeNotification(null, item.Item, NotificationsType.Compendium);
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
                Notifications.instance.SetNewNotification($"{item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);
            }
        }
            
    }
}
