using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    [SerializeField]
    private UIScreenType screenType;

    public UIScreenType GetScreenType()
    {
        return screenType;
    }
}
