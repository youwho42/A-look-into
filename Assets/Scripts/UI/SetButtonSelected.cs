using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SetButtonSelected : MonoBehaviour
{
    public GameObject buttonSelectedAtStart;
    private void OnEnable()
    {
        if(buttonSelectedAtStart != null)
            SetSelectedButton(buttonSelectedAtStart);
    }

    public void SetSelectedButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonSelectedAtStart);
    }
}
