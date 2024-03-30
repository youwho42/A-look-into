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

    //public TextMeshProUGUI undertakingTitle;
    public TextMeshProUGUI undertakingDescription;

    List<UndertakingsButton> undertakingsButtons = new List<UndertakingsButton>();
    private void Start()
    {
        GameEventManager.onUndertakingsUpdateEvent.AddListener(SetAvailableUndertakings);
        SetAvailableUndertakings();
        ClearCurrentUndertaking();
    }
    
    private void OnDestroy()
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

        var go = Instantiate(undertakingsSpace, undertakingsButtonHolder.transform);
        go.SetActive(activeUndertakings.Count > 0);
        // create undertaking buttons of complete undertakings
        
        var reverso = activeUndertakings;
        reverso.Reverse();
        foreach (var undertaking in reverso)
        {
            if (undertaking.CurrentState == UndertakingState.Complete)
                CreateUndertakingButton(undertaking);
        }

        //ClearCurrentUndertaking();
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
        //undertakingTitle.text = currentUndertaking.localizedName.GetLocalizedString();
        

        string desc = undertaking.CurrentState == UndertakingState.Complete ? currentUndertaking.localizedCompleteDescription.GetLocalizedString() : currentUndertaking.localizedDescription.GetLocalizedString();
        //undertakingDescription.text = $"{desc}<br>{tasks}";
        undertakingDescription.text = $"\n<style=\"H1\">{currentUndertaking.localizedName.GetLocalizedString()}</style>\n\n{desc}\n\n";
    }

    public void ClearCurrentUndertaking()
    {
        currentUndertaking = null;
        
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
