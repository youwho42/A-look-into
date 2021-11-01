using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPrompt : MonoBehaviour
{
    public Canvas promptCanvas;
    bool isDisplayed;


    private void Start()
    {
        promptCanvas.enabled = false;
        promptCanvas.worldCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            promptCanvas.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            promptCanvas.enabled = false;
        }
    }
}
