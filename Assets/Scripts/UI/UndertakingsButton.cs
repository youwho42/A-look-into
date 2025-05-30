using Klaxon.UndertakingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UndertakingsButton : MonoBehaviour
{
    [HideInInspector]
    public UndertakingObject undertaking;
    public TextMeshProUGUI undertakingName;
    public Button button;
    public bool isCompleted;
    public Color incompleteColor;
    public Color completedColor;
    public void SetCurrentUndertaking()
    {
        UndertakingsDisplayUI.instance.SetCurrentUndertaking(undertaking);
    }

    public void AddUndertaking(UndertakingObject newUndertaking)
    {
        undertaking = newUndertaking;
        undertaking.localizedName.StringChanged += ResetName;
        undertakingName.text = undertaking.localizedName.GetLocalizedString();
        isCompleted = undertaking.CurrentState == UndertakingState.Complete;
        var ac = button.colors;
        ac.normalColor = isCompleted ? completedColor : incompleteColor;
        button.colors = ac;

    }
    void ResetName(string name)
    {
        undertakingName.text = name;
    }

    public void Clear()
    {
        undertaking = null;
        undertakingName.text = "";
        isCompleted = false;
        var ac = button.colors;
        ac.normalColor = incompleteColor;
        button.colors = ac;
    }


}
