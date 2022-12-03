using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public List<GameObject> screens = new List<GameObject>();

   
    private UIScreenType currentScreen;

    public bool canChangeUI;

    private void Start()
    {
        
        canChangeUI = true;
        foreach (GameObject go in screens)
        {
           
            go.SetActive(false);
            
        }
        DisplayScreen(UIScreenType.StartScreen);
    }

    public void DisplayPlayerUI()
    {
        foreach (var s in screens)
        {
            if (s.GetComponent<UIScreen>().GetScreenType() == UIScreenType.PlayerUI)
            {
                s.SetActive(true);
                
            }
        }
    }

    public void DisplayScreen(UIScreenType screen)
    {
        if (!canChangeUI)
            return;
        foreach (var s in screens)
        {
            if(s.GetComponent<UIScreen>().GetScreenType() == screen)
            {
                s.SetActive(true);
                currentScreen = screen;
                
            }
            else
            {
                s.SetActive(false);
            }
        }
       
    }

    public void HideScreens(UIScreenType screenType)
    {
        foreach (var s in screens)
        {
            if (s.GetComponent<UIScreen>().GetScreenType() == screenType)
            {
                s.SetActive(false);
            }
        }
    }

    public void HideAllScreens()
    {
        foreach (var s in screens)
        {
            currentScreen = UIScreenType.None;
            s.SetActive(false);
        }
    }

    public UIScreenType CurrentUIScreen()
    {
        return currentScreen;
    }
}
