using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using QuantumTek.QuantumQuest;
using UnityEngine;

public class StartGatherQuest : MonoBehaviour
{
    public QQ_QuestHandler handler;
    public string questName;
    QQ_Quest quest;

    QQ_Task currentTask;
    public QI_Inventory inventory;
    

    bool isActive;

    public void StartQuest()
    {
        if (quest == null)
        {

            
            
            handler.AssignQuest(questName);
            handler.ActivateQuest(questName);
            quest = handler.GetQuest(questName);
            Debug.Log(quest.Status);


        }
    }
    public void TurnInQuest()
    {
        if (inventory.GetStock("Flower") >= handler.GetTask(questName, "Pick flower").MaxProgress)
        {
            handler.ProgressTask(questName, "Pick flower", handler.GetTask(questName, "Pick flower").MaxProgress);
            inventory.RemoveItem("Flower", (int)handler.GetTask(questName, "Pick flower").MaxProgress);

        }
    }
    public bool IsQuestComplete()
    {
        return quest.Completed;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {

    //        if (quest == null)
    //        {

    //            inventory = collision.GetComponent<QI_Inventory>();
    //            handler = collision.GetComponent<QQ_QuestHandler>();
    //            handler.AssignQuest(questName);
    //            handler.ActivateQuest(questName);
    //            quest = handler.GetQuest(questName);



    //        }
    //        if (quest.Completed)
    //            return;


    //        if(inventory.GetStock("Flower") > 0)
    //        {
    //            handler.ProgressTask(questName, "Pick flower", 1);
    //            inventory.RemoveItem("Flower", 1);

    //            if (handler.GetQuest(questName).Completed)
    //            {
    //                Debug.Log("Quest complete!");
    //            }


    //        }
    //    }
    //}
}
