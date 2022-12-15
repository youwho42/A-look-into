using QuantumTek.QuantumInventory;
using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation instance;

    public Transform player;
    public GravityItemMovementControllerNew playerController;
    public QI_Inventory playerInventory;
    public EquipmentManager equipmentManager;
    public QI_CraftingRecipeDatabase playerRecipeDatabase;
    public QI_ItemDatabase playerResourceCompendiumDatabase;
    public QI_ItemDatabase playerAnimalCompendiumDatabase;
    public QI_ItemDatabase playerNotesCompendiumDatabase;
    public bool uiScreenVisible;
    
    public PlayerInput playerInput;

    public PlayerStats playerStats;
    public CurrentTilePosition currentTilePosition;
    public Animator playerAnimator;

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

    private void Start()
    {
        currentTilePosition = player.GetComponent<CurrentTilePosition>();
        playerAnimalCompendiumDatabase.Items.Clear();
        playerResourceCompendiumDatabase.Items.Clear();
        playerNotesCompendiumDatabase.Items.Clear();
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
            if (!PlayerInformation.instance.playerResourceCompendiumDatabase.Items.Contains(item.Item))
            {
                PlayerInformation.instance.playerResourceCompendiumDatabase.Items.Add(item.Item);
                NotificationManager.instance.SetNewNotification($"{item.Item.Name} was added to your resources compendium.");
                GameEventManager.onResourceCompediumUpdateEvent.Invoke();
            }
        }
        
    }

}
