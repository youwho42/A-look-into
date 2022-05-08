using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuitUI : MonoBehaviour
{

    public GameObject quitUI;

    private void Start()
    {
        quitUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleUI();
    }

    public void ToggleUI()
    {
        quitUI.SetActive(!quitUI.activeSelf);
    }

    

    public void QuitGame()
    {
        Application.Quit();
    }



}
