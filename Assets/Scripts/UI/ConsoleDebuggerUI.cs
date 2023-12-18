using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleDebuggerUI : MonoBehaviour
{
    public static ConsoleDebuggerUI instance;

    private void Awake()
    {
        if(instance == null)
            instance = this; 
    }

    public TextMeshProUGUI debuggerText;

    public void SetDebuggerText(string text)
    {
        debuggerText.text = text;
    }
}
