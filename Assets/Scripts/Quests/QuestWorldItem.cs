using Klaxon.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestState
{
    Inactive,
    Active,
    Complete
}

public class QuestWorldItem : MonoBehaviour
{
    public QuestObject quest;
    public List<TaskWorldItem> tasks;
    public QuestState CurrentState;

    private void Start()
    {
        for (int i = 0; i < quest.Tasks.Count; i++)
        {
            var go = new GameObject();
            go.transform.parent = transform;
            go.name = quest.Tasks[i].Name;
            var goTask = go.AddComponent<TaskWorldItem>();
            goTask.task = quest.Tasks[i];
            tasks.Add(goTask);
        }
    }
    

    public void ActivateQuest()
    {
        if (CurrentState == QuestState.Complete)
            return;
        CurrentState = QuestState.Active;


    }
    public void TryCompleteQuest()
    {
        bool complete = true;
        foreach (var task in tasks)
        {
            if (task.IsComplete)
                continue;
            complete = false;
        }
        if (complete)
            CurrentState = QuestState.Complete;
        Debug.Log($"Quest {quest.Name} {CurrentState}");
    }

    
    
}
