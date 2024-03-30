using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetButtonSelected : MonoBehaviour
{
    

    public Button firstButtonSelected;
    public GameObject buttonHolder;

  
    public void SetSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (buttonHolder != null && firstButtonSelected == null)
        {
            firstButtonSelected = buttonHolder.GetComponentInChildren<Button>();
        }
        if (firstButtonSelected != null)
            EventSystem.current.SetSelectedGameObject(firstButtonSelected.gameObject);
    }

    
}
