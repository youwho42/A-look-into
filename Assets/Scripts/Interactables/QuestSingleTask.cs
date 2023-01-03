using QuantumTek.QuantumInventory;
using QuantumTek.QuantumQuest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSingleTask : MonoBehaviour
{

    
    public QQ_QuestSO quest;


    public void AddQuest()
    {
        if (quest != null)
        {
            PlayerInformation.instance.playerQuestHandler.AssignQuest(quest.Quest.Name);
            PlayerInformation.instance.playerQuestHandler.ActivateQuest(quest.Quest.Name);
        }
    }
    public void TurnInQuest()
    {
        
        if (!PlayerInformation.instance.playerQuestHandler.GetQuest(quest.Quest.Name).Completed)
        {
            
            PlayerInformation.instance.playerQuestHandler.CompleteQuest(quest.Quest.Name);
        }
        
    }
}
