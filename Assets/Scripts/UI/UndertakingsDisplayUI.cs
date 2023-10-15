using Klaxon.UndertakingSystem;

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
    public UndertakingObject currentUndertaking;

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
        var activeUndertakings = PlayerInformation.instance.playerUndertakings.activeUndertakings;

        // create undertaking buttons of NOT complete undertakings
        foreach (var undertaking in activeUndertakings)
        {
            if (undertaking.CurrentState == UndertakingState.Active)
                CreateUndertakingButton(undertaking);
        }

        Instantiate(undertakingsSpace, undertakingsButtonHolder.transform);
        // create undertaking buttons of complete undertakings
        
        var reverso = activeUndertakings;
        reverso.Reverse();
        foreach (var undertaking in reverso)
        {
            if (undertaking.CurrentState == UndertakingState.Complete)
                CreateUndertakingButton(undertaking);
        }

        ClearCurrentUndertaking();
    }

    void CreateUndertakingButton(UndertakingObject undertaking)
    {
        UndertakingsButton newUndertaking = Instantiate(undertakingsButton, undertakingsButtonHolder.transform);
        undertakingsButtons.Add(newUndertaking);
        newUndertaking.AddUndertaking(undertaking);
    }

    public void SetCurrentUndertaking(UndertakingObject undertaking)
    {
        ClearCurrentUndertaking();
        currentUndertaking = undertaking;
        undertakingTitle.text = currentUndertaking.localizedName.GetLocalizedString();
        //string tasks = "";

        //if (undertaking.CurrentState != UndertakingState.Complete)
        //{
        //    for (int i = 0; i < undertaking.Tasks.Count; i++)
        //    {
        //        tasks += $"\n-{undertaking.Tasks[i].Name}";
        //    }
        //}

        string desc = undertaking.CurrentState == UndertakingState.Complete ? currentUndertaking.localizedCompleteDescription.GetLocalizedString() : currentUndertaking.localizedDescription.GetLocalizedString();
        //undertakingDescription.text = $"{desc}<br>{tasks}";
        undertakingDescription.text = $"{desc}";
    }

    public void ClearCurrentUndertaking()
    {
        currentUndertaking = null;
        undertakingTitle.text = "";
        undertakingDescription.text = "";
    }



    public void ClearUndertakingsButtons()
    {
        foreach (Transform child in undertakingsButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        undertakingsButtons.Clear();
    }
}
