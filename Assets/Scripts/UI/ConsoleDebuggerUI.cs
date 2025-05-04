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

    Queue<string> debugQueue = new Queue<string>();
    bool debugging;
    private void Start()
    {
        debuggerText.gameObject.SetActive(false);
    }
    public void SetDebuggerText(string text)
    {
        debugQueue.Enqueue(text);
        if (!debugging)
            StartCoroutine("SetTextCo");
    }

    IEnumerator SetTextCo()
    {
        debugging = true;
        while(debugQueue.Count > 0)
        {
            debuggerText.gameObject.SetActive(true);
            debuggerText.text = debugQueue.Dequeue();
            yield return new WaitForSeconds(3.0f);
            
        }
        ResetText();
        debugging= false;
        yield return null;
    }
    void ResetText()
    {
        debuggerText.text = "";
        debuggerText.gameObject.SetActive(false);
    }
}
