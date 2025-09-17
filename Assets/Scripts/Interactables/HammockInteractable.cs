using UnityEngine;
using System.Collections.Generic;
using Klaxon.Interactable;
using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;

public class HammockInteractable : Interactable
{

    
    PlayerInformation player;
    QI_ItemData item;
    //public bool facingRight;
    public StatChanger gumptionChanger;
    public StatChanger bounceChanger;
    public StatModifier gumptionModifier;
    public StatModifier bounceModifier;
    RealTimeDayNightCycle dayNightCycle;
    CallbackOnTick currentCallback;

    public override void Start()
    {
        base.Start();
        item = GetComponent<QI_Item>().Data;
        player = PlayerInformation.instance;
        dayNightCycle = RealTimeDayNightCycle.instance;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            ViewSky();
        else if(UIScreenManager.instance.GetCurrentUI() == UIScreenType.Sky)
            HideSky();
    }

    public override void LongInteract(GameObject interactor)
    {
        base.LongInteract(interactor);
        if (player.playerInventory.AddItem(item, 1, false))
        {
            player.statHandler.RemoveModifiableModifier(item.placementGumption);
            Destroy(gameObject);
        }
            
        
    }

    

    private void ViewSky()
    {
        CreateStarField.instance.CreateStarMap();
        UIScreenManager.instance.DisplayIngameUI(UIScreenType.Sky, true);
        UIScreenManager.instance.DisplayPlayerHUD(false);
        GameEventManager.onTimeTickEvent.AddListener(CheckAddStats);
        SkyUI.instance.SetHammock(this);
        currentCallback = dayNightCycle.AddCallbackOnTick(AddSpecials, dayNightCycle.GetCycleTime(60));
    }

    
    public void HideSky()
    {
        UIScreenManager.instance.HideScreenUI();
        GameEventManager.onTimeTickEvent.RemoveListener(CheckAddStats);
        dayNightCycle.RemoveCallbackOnTick(currentCallback);
        currentCallback = null;
    }

    void CheckAddStats(int tick)
    {
        PlayerInformation.instance.statHandler.ChangeStat(gumptionChanger);
        PlayerInformation.instance.statHandler.ChangeStat(bounceChanger);
    }

    void AddSpecials()
    {
        player.statHandler.AddModifiableModifier(bounceModifier);
        player.statHandler.AddModifiableModifier(gumptionModifier);

    }
}
