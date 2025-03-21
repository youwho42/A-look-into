using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using QuantumTek.QuantumInventory;
using Klaxon.StatSystem;
using System.Linq;

public class ConsoleDisplayUI : MonoBehaviour
{
    public GameObject textAndButton;
    public TMP_Dropdown itemsDropdownField;
    public TMP_Dropdown itemAmountDropdownField;
    public TMP_Dropdown locationsDropdownField;
    public QI_ItemDatabase allItemsDatabase;
    public Transform locationHolder;
    List<Transform> locations = new List<Transform>();
    public TMP_Dropdown timesDropdownField;
    public StatChanger agencyChanger;
    public int lastSelectedItem = 0;

    private void Start()
    {
        SetUpItems();
        SetUpLocations();
        SetUpTimes();
    }

    void SetUpTimes()
    {
        timesDropdownField.ClearOptions();
        List<string> times = new List<string>();
        
        for (int i = 0; i < 24; i++)
        {
            times.Add(i.ToString());
        }

        timesDropdownField.AddOptions(times);
    }


    void SetUpItems()
    {
        itemsDropdownField.ClearOptions();
        List<string> items = new List<string>();
        items.Add(" ");
        for (int i = 0; i < allItemsDatabase.Items.Count; i++)
        {
            if (allItemsDatabase.Items[i] == null)
                continue;
            items.Add(allItemsDatabase.Items[i].Name);
        }
        items.Sort();
        itemsDropdownField.AddOptions(items);
    }
    void SetUpLocations()
    {
        locations.Clear();
        locations = locationHolder.GetComponentsInChildren<Transform>().Where(t => t != locationHolder).ToList();
        locationsDropdownField.ClearOptions();
        List<string> locs = new List<string>();
        locs.Add(" ");
        for (int i = 0; i < locations.Count; i++)
        {
            locs.Add(locations[i].name);
        }
        locationsDropdownField.AddOptions(locs);
    }
    public void ToggleConsole()
    {
        textAndButton.SetActive(!textAndButton.activeSelf);
        PlayerInformation.instance.TogglePlayerInput(!textAndButton.activeSelf);
        PlayerInformation.instance.playerInput.isPaused = textAndButton.activeSelf;
    }

    public void AddToInventory()
    {
        lastSelectedItem = itemsDropdownField.value;
        var item = itemsDropdownField.options[itemsDropdownField.value].text;
        var amount = int.Parse(itemAmountDropdownField.options[itemAmountDropdownField.value].text);
        var i = allItemsDatabase.GetItem(item);
        if (i == null && amount == 0)
            return;

        PlayerInformation.instance.playerInventory.AddItem(i, amount, false);

    }

    public void GoToPosition()
    {
        var loc = locationsDropdownField.options[locationsDropdownField.value].text;
        if (loc == " ")
            return;
        foreach (var position in locations)
        {
            if (position.gameObject.name == loc)
            {
                PlayerInformation.instance.player.position = position.position;
                PlayerInformation.instance.currentTilePosition.position = PlayerInformation.instance.currentTilePosition.GetCurrentTilePosition(position.position);
                PlayerInformation.instance.playerController.currentLevel = (int)position.position.z - 1;
                break;
            }

        }
        
    }

    public void SetTime()
    {
        int time = int.Parse(timesDropdownField.options[timesDropdownField.value].text);
        time *= 60;
        RealTimeDayNightCycle.instance.currentTimeRaw = time-1;
    }

    public void AddAgency()
    {
        PlayerInformation.instance.statHandler.ChangeStat(agencyChanger);
    }
    public void AddSparks()
    {
        PlayerInformation.instance.purse.AddToPurse(1000);
    }
    public void AddAllRecipes()
    {
        var allRecipes = AllItemsDatabaseManager.instance.allRecipesDatabase;
        var player = PlayerInformation.instance.playerRecipeDatabase;
        foreach (var item in allRecipes.CraftingRecipes)
        {
            player.CraftingRecipes.Add(item);
        }
    }
}
