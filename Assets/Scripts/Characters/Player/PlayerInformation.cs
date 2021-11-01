using QuantumTek.QuantumInventory;
using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation instance;

    public Transform player;
    public QI_Inventory playerInventory;
    public QI_CraftingRecipeDatabase playerRecipeDatabase;
    
    public bool uiScreenVisible;
    
    public PlayerInput playerInput;

    public PlayerStats playerStats;

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


}
