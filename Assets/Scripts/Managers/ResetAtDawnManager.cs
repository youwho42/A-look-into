using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResetAtDawnManager : MonoBehaviour
{

    public static ResetAtDawnManager instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
           
    }
    private void OnEnable()
    {
        GameEventManager.onTimeHourEvent.AddListener(ResetAllItems);
        GameEventManager.onGameStartLoadEvent.AddListener(ResetManager);
        ResetAllItems(hourToDailySpawn);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(ResetAllItems);
        GameEventManager.onGameStartLoadEvent.RemoveListener(ResetManager);
    }

    public List<IResetAtDawn> itemsToReset = new List<IResetAtDawn>();

    int hourToDailySpawn = 5;

    //public void AddToManager(IResetAtDawn item)
    //{
    //    itemsToReset.Add(item);
    //}
    void ResetManager()
    {
        itemsToReset.Clear();
    }
    public void ResetAllItems(int time)
    {
        
        if (time != hourToDailySpawn)
            return;
        itemsToReset = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IResetAtDawn>().ToList();
        foreach (var item in itemsToReset)
        {
            item.ResetAtDawn();
        }
    }
}
