using QuantumTek.QuantumInventory;
using QuantumTek.QuantumQuest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.ComponentModel.Design.ObjectSelectorEditor;

public class UndertakingsButton : MonoBehaviour
{
    QQ_Quest undertaking;
    public TextMeshProUGUI undertakingName;
    //CraftingStationDisplayUI craftingStation;
    public Button button;
    public bool isCompleted;
    public Color incompleteColor;
    public Color completedColor;
    public void SetCurrentUndertaking()
    {
        UndertakingsDisplayUI.instance.SetCurrentUndertaking(undertaking);
    }

    public void AddUndertaking(QQ_Quest newUndertaking)
    {
        undertaking = newUndertaking;
        undertakingName.text = undertaking.Name;
        isCompleted = undertaking.Completed;
        var ac = button.colors;
        ac.normalColor = undertaking.Completed ? completedColor : incompleteColor;
        button.colors = ac;

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
