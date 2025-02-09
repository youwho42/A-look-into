using Klaxon.UndertakingSystem;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        if (currentUndertaking != null)
            SetCurrentUndertaking(currentUndertaking);
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
        

        string desc = undertaking.CurrentState == UndertakingState.Complete ? currentUndertaking.localizedCompleteDescription.GetLocalizedString() : currentUndertaking.localizedDescription.GetLocalizedString();
        string tasks = "";
        
        var allTasks = AllTasksDictionary(undertaking);
        foreach (var task in allTasks)
        {
            string quantText = "";
            var quant = task.Value;
            if(quant.y > 1)
                quantText = $"{quant.x}/{quant.y}";
            tasks += quant.x == quant.y ? "" : $"{task.Key} {quantText}\n";
        }

        undertakingDescription.text = $"\n<style=\"H1\">{currentUndertaking.localizedName.GetLocalizedString()}</style>\n\n{desc}\n\n{tasks}";
    }

    public void ClearCurrentUndertaking()
    {
        currentUndertaking = null;
        
        undertakingDescription.text = " ";
    }



    public void ClearUndertakingsButtons()
    {
        foreach (Transform child in undertakingsButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        undertakingsButtons.Clear();
    }

    
    Dictionary<string, Vector2Int> AllTasksDictionary(UndertakingObject undertaking)
    {
        Dictionary<string, Vector2Int> allTasks = new Dictionary<string, Vector2Int>();
        foreach (var task in undertaking.Tasks)
        {
            if (allTasks.ContainsKey(task.localizedDescription.GetLocalizedString()))
            {
                allTasks[task.localizedDescription.GetLocalizedString()] += new Vector2Int(task.IsComplete ? 1 : 0, 1);
            }
            else
            {
                Vector2Int quantities = new Vector2Int(task.IsComplete ? 1 : 0, 1);
                allTasks.Add(task.localizedDescription.GetLocalizedString(), quantities);
            }
        }
        return allTasks;
    }
}
