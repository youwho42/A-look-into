using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUiDisplay : Interactable
{
    public GameObject craftingUI;
    public CraftingTable craftingTable;

    public override void Start()
    {
        base.Start();
        craftingUI.SetActive(false);
    }
    private void ActivateCrafting()
    {
        craftingUI.SetActive(!craftingUI.activeSelf);

        craftingTable.ResetButtons();
        craftingTable.SetAvailableRecipes();

        PlayerInformation.instance.TogglePlayerInput(!craftingUI.activeSelf);
    }
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        
        ActivateCrafting();
        hasInteracted = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && craftingUI.activeSelf)
        {
            ActivateCrafting();
        }
    }
}
