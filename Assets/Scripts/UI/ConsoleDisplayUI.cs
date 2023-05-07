using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using QuantumTek.QuantumInventory;
using UnityEngine.UIElements;

public class ConsoleDisplayUI : MonoBehaviour
{
    public GameObject textAndButton;
    public TMP_Dropdown itemsDropdownField;
    public TMP_Dropdown itemAmountDropdownField;
    public TMP_Dropdown locationsDropdownField;
    public QI_ItemDatabase allItemsDatabase;
    public List<Transform> locations = new List<Transform>();

    private void Start()
    {
        SetUpItems();
        SetUpLocations();
    }
    void SetUpItems()
    {
        itemsDropdownField.ClearOptions();
        List<string> items = new List<string>();
        items.Add(" ");
        for (int i = 0; i < allItemsDatabase.Items.Count; i++)
        {
            items.Add(allItemsDatabase.Items[i].Name);
        }
        items.Sort();
        itemsDropdownField.AddOptions(items);
    }
    void SetUpLocations()
    {
        locationsDropdownField.ClearOptions();
        List<string> locs = new List<string>();
        locs.Add(" ");
        for (int i = 0; i < locations.Count; i++)
        {
            locs.Add(locations[i].name);
        }
        locs.Sort();
        locationsDropdownField.AddOptions(locs);
    }
    public void ToggleConsole()
    {
        textAndButton.SetActive(!textAndButton.activeSelf);
        PlayerInformation.instance.TogglePlayerInput(!textAndButton.activeSelf);
        PlayerInformation.instance.playerInput.isPaused = textAndButton.activeSelf;
    }

    //public void DoTheThing()
    //{
    //    if (textInputField.text == "")
    //        return;
    //    string[] myStringSplit = textInputField.text.Split(':');

    //    if (myStringSplit[0] == "Add")
    //        AddToInventory(myStringSplit[1], int.Parse(myStringSplit[2]));
    //    else if (myStringSplit[0] == "GoTo")
    //        GoToPosition(myStringSplit[1]);
    //    else
    //        textInputField.text = "No valid command input";


    //}

    public void AddToInventory()
    {
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
                break;
            }

        }
        
    }
}
