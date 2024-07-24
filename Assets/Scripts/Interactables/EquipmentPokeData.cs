using Klaxon.StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/PokeItem", fileName = "New Poke Item")]
public class EquipmentPokeData : EquipmentData
{
    public StatChanger statChanger;


    public override void UseEquippedItem()
    {
        var poke = PlayerInformation.instance.playerPoke;
        if (poke.canPoke)
        {
            if (InteractCostReward())
            {
                if ((int)GameDificulty >= (int)poke.CurrentPokable.GameDificulty)
                    MiniGameManager.instance.StartMiniGame(MiniGameType.Poking, poke.CurrentPokable);
                else
                    Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Wrong equipment"), null, 0, NotificationsType.Warning);

            }
        }

    }

    


    bool InteractCostReward()
    {
        if (PlayerInformation.instance.statHandler.GetStatCurrentModifiedValue("Bounce") >= Mathf.Abs(statChanger.Amount))
        {
            PlayerInformation.instance.statHandler.ChangeStat(statChanger);
            return true;
        }
        PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
        Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Missing bounce"), null, 0, NotificationsType.Warning);

        return false;
    }


}
