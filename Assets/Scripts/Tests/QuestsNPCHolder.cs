using QuantumTek.QuantumInventory;
using QuantumTek.QuantumQuest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsNPCHolder : MonoBehaviour
{
    public QQ_QuestSO[] quests;
   

    public bool TurnInQuest(QQ_Quest quest)
    {
        // this is a boolean quest, only one thing to do, is it be done?
        if (quest.BooleanQuest)
        {
            if (!PlayerInformation.instance.playerQuestHandler.GetQuest(quest.Name).Completed)
            {
                GiveRewards(quest);
                PlayerInformation.instance.playerQuestHandler.CompleteQuest(quest.Name);
                GameEventManager.onUndertakingsUpdateEvent.Invoke();
                return true;
            }
        }
        bool hasAllMaterials = true;
        for (int i = 0; i < quest.Tasks.Count; i++)
        {
            if(PlayerInformation.instance.playerInventory.GetStock(quest.Tasks[i].TaskItem) < quest.Tasks[i].MaxProgress)
            {
                hasAllMaterials = false;
                break;
            }
            
        }
        
        if(hasAllMaterials)
        {
            RemoveItemsFromInventory(quest);
            GiveRewards(quest);
            PlayerInformation.instance.playerQuestHandler.CompleteQuest(quest.Name);
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
        }
            

        return hasAllMaterials;
    }

    
    void RemoveItemsFromInventory(QQ_Quest quest)
    {
        for (int i = 0; i < quest.Tasks.Count; i++)
        {
            PlayerInformation.instance.playerInventory.RemoveItem(quest.Tasks[i].TaskItem, (int)quest.Tasks[i].MaxProgress);
            GameEventManager.onInventoryUpdateEvent.Invoke();
        }
    }

    void GiveRewards(QQ_Quest quest)
    {
        PlayerInformation.instance.playerStats.AddToStat("Gumption", quest.GumptionReward);
        GameEventManager.onStatUpdateEvent.Invoke();
        if (quest.ItemReward != "")
            PlayerInformation.instance.playerInventory.AddItem(AllItemsDatabaseManager.instance.allItemsDatabase.GetItem(quest.ItemReward), quest.ItemAmount, false);
    }
}
