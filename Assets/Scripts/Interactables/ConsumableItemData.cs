using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Klaxon.StatSystem;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/ConsumableItem", fileName = "New Consumable Item")]
public class ConsumableItemData : QI_ItemData
{

    public List<StatChanger> statChangers = new List<StatChanger>();
    public List<StatModifier> statModifiers = new List<StatModifier>();
    public QI_ItemData itemToBecome;
    PlayerInformation player;
    public string soundName;
    public override void UseItem()
    {
        base.UseItem();
        ConsumeItem();
    }
    void ConsumeItem()
    {
        player = PlayerInformation.instance;
        bool isBounce = false;
        foreach (var mod in statModifiers)
        {
            player.statHandler.AddModifier(mod);
        }
        foreach (var mod in statChangers)
        {
            player.statHandler.ChangeStat(mod);
            if (mod.StatToModify.Name == "Bounce")
                isBounce = true;
        }

        PlaySound(isBounce);

        player.playerInventory.RemoveItem(this.Name, 1);
        if(itemToBecome != null)
        {
            if(!player.playerInventory.AddItem(itemToBecome, 1, false))
                LostAndFoundManager.instance.inventory.AddItem(itemToBecome, 1, false);
            
        }
             
    }

    void PlaySound(bool bounce)
    {
        if (bounce)
        {
            bool full = player.statHandler.GetStatCurrentModifiedValue("Bounce") >= player.statHandler.GetStatMaxModifiedValue("Bounce");
            AudioManager.instance.PlaySound("ConsumeBounceWarning");
            
            return;
        }
        if (soundName !="")
            AudioManager.instance.PlaySound(soundName);    
        
    }
}
