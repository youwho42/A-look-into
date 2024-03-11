using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScreenUI : MonoBehaviour
{
    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.LoadScreenUI);
    }
}
