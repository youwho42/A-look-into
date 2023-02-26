using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputFocusHelper : MonoBehaviour, ISelectHandler
{
    [SerializeField] TMP_InputField inputField;
    void Reset()
    {
        inputField = GetComponent<TMP_InputField>();
    }
    void OnEnable()
    {
        GameEventManager.onSubmitEvent.AddListener(ToggleSelect);
    }

    void OnDisable()
    {
        GameEventManager.onSubmitEvent.RemoveListener(ToggleSelect);
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(UnFocusByDefault(false));
    }

    void ToggleSelect()
    {
        if(inputField.isFocused)
            StartCoroutine(UnFocusByDefault(!inputField.IsActive()));
    }

    IEnumerator UnFocusByDefault(bool activate)
    {
        yield return new WaitForEndOfFrame();
        if(!activate)
            inputField.DeactivateInputField();
        else
            inputField.ActivateInputField();
    }

    
}