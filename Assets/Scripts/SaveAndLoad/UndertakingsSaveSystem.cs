using QuantumTek.QuantumInventory;
using QuantumTek.QuantumQuest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndertakingsSaveSystem : MonoBehaviour, ISaveable
{
    public QQ_QuestHandler playerUndertakings;


    public object CaptureState()
    {
        List<string> names = new List<string>();
        List<bool> states = new List<bool>();
        foreach (var undertaking in playerUndertakings.Quests)
        {
            if (undertaking.Value.Status == QQ_QuestStatus.Active || undertaking.Value.Status == QQ_QuestStatus.Completed)
            {
                names.Add(undertaking.Value.Name);
                states.Add(undertaking.Value.Completed);
            }
                
            
        }

        return new SaveData
        {
            undertakingName = names,
            isComplete = states
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        for (int i = 0; i < saveData.undertakingName.Count; i++)
        {
            playerUndertakings.ActivateQuest(saveData.undertakingName[i], true);
            if (saveData.isComplete[i]==true)
                playerUndertakings.CompleteQuest(saveData.undertakingName[i], true);
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
        }
    }

    [Serializable]
    private struct SaveData
    {
        public List<string> undertakingName;
        public List<bool> isComplete;

    }
}
