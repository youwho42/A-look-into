using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{


    public void StartNewGame()
    {
        UIScreenManager.instance.DisplayWarning("New Game Warning", UIScreenType.CharacterSelectUI);
        
    }

    public void ViewLoadGameUI()
    {
        UIScreenManager.instance.DisplayLoadGameUI(true, UIScreenType.MainMenuUI);
    }

    public void ViewOptions()
    {
        UIScreenManager.instance.DisplayOptionsUI(true, UIScreenType.MainMenuUI);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
