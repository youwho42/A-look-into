using QuantumTek.QuantumQuest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UndertakingsDisplayUI : MonoBehaviour
{
    public static UndertakingsDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    public GameObject undertakingsButtonHolder;
    public UndertakingsButton undertakingsButton;
    public GameObject undertakingsSpace;
    public QQ_Quest currentUndertaking;
    

    public TextMeshProUGUI undertakingTitle;
    public TextMeshProUGUI undertakingDescription;

    List<UndertakingsButton> undertakingsButtons = new List<UndertakingsButton>();
    private void Start()
    {
        GameEventManager.onUndertakingsUpdateEvent.AddListener(SetAvailableUndertakings);
    }
    
    private void OnDisable()
    {
        GameEventManager.onUndertakingsUpdateEvent.RemoveListener(SetAvailableUndertakings);
    }

    public void SetAvailableUndertakings()
    {
        ClearUndertakingsButtons();
        var undertakings = PlayerInformation.instance.playerQuestHandler.Quests;
        
        // create undertaking buttons of NOT complete undertakings
        foreach (var undertaking in undertakings)
        {
            if (undertaking.Value.Status == QQ_QuestStatus.Inactive || undertaking.Value.Status == QQ_QuestStatus.Completed)
                continue;
            CreateUndertakingButton(undertaking.Value);
        }
        
        Instantiate(undertakingsSpace, undertakingsButtonHolder.transform);
        // create undertaking buttons of complete undertakings
        foreach (var undertaking in undertakings)
        {
            if (undertaking.Value.Status == QQ_QuestStatus.Inactive || undertaking.Value.Status == QQ_QuestStatus.Active)
                continue;
            CreateUndertakingButton(undertaking.Value);
        }

        ClearCurrentUndertaking();
    }

    void CreateUndertakingButton(QQ_Quest undertaking)
    {
        UndertakingsButton newUndertaking = Instantiate(undertakingsButton, undertakingsButtonHolder.transform);
        undertakingsButtons.Add(newUndertaking);
        newUndertaking.AddUndertaking(undertaking);
    }

    public void SetCurrentUndertaking(QQ_Quest undertaking)
    {
        ClearCurrentUndertaking();
        currentUndertaking = undertaking;
        undertakingTitle.text = currentUndertaking.Name;
        string tasks = "";
        if (!undertaking.BooleanQuest)
        {
            if (!undertaking.Completed)
            {
                for (int i = 0; i < undertaking.Tasks.Count; i++)
                {
                    tasks += $"\n-{undertaking.Tasks[i].TaskItem} {PlayerInformation.instance.playerInventory.GetStock(undertaking.Tasks[i].TaskItem)}/{undertaking.Tasks[i].MaxProgress}";
                }
            }
        }
        string desc = undertaking.Completed ? currentUndertaking.CompletedDescription : currentUndertaking.Description;
        undertakingDescription.text = $"{desc}<br>{tasks}";
    }

    public void ClearCurrentUndertaking()
    {
        currentUndertaking = null;
        undertakingTitle.text = "";
        undertakingDescription.text = "";
    }



    public void ClearUndertakingsButtons()
    {
        while (undertakingsButtonHolder.transform.childCount > 0)
        {
            DestroyImmediate(undertakingsButtonHolder.transform.GetChild(0).gameObject);
        }
        undertakingsButtons.Clear();
    }
}
