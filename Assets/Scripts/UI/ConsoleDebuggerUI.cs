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
    private void Start()
    {
        debuggerText.gameObject.SetActive(false);
    }
    public void SetDebuggerText(string text)
    {
        debuggerText.gameObject.SetActive(true);
        debuggerText.text = text;
        Invoke("ResetText", 5f);
    }

    void ResetText()
    {
        debuggerText.text = "";
        debuggerText.gameObject.SetActive(false);
    }
}
