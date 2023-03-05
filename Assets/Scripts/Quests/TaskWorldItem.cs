using Klaxon.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskWorldItem : MonoBehaviour
{
    public TaskObject task;
    public bool IsComplete;

    public void CompleteTask()
    {
        IsComplete = true;
    
    }

    
}
