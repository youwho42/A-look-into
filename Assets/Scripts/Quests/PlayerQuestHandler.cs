using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.QuestSystem
{
    public class PlayerQuestHandler : MonoBehaviour
    {
        public List<QuestWorldItem> playerQuests = new List<QuestWorldItem>();

        public void AddQuest(QuestWorldItem quest)
        {
            if (!playerQuests.Contains(quest))
            {
                playerQuests.Add(quest);
                quest.ActivateQuest();
            }
                
        }

        public void TryCompleteTask(QuestObject quest, TaskObject task)
        {
            
            foreach (var playerQuest in playerQuests)
            { 
                if (playerQuest.quest == quest)
                {
                    for (int i = 0; i < playerQuest.quest.Tasks.Count; i++)
                    {
                        if (playerQuest.quest.Tasks[i] == task)
                        {
                            playerQuest.tasks[i].CompleteTask();
                            playerQuest.TryCompleteQuest();
                        }
                    }
                }
            }
        }
    }
}
